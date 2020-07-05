/*
 * Initially based on Box2D by Erin Catto, license follows;
 *
 * Copyright (c) 2009 Erin Catto http://www.box2d.org
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software
 * in a product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using InlineIL;
using JetBrains.Annotations;
using Math = CannyFastMath.Math;

namespace Fizix {

  [PublicAPI]
  public abstract partial class BoxTree {

    public const int MinimumCapacity = 16;

    protected readonly float BoxNodeGrowth;

    protected readonly Func<int, int> GrowthFunc;

    private readonly ReaderWriterLockSlim? _lock;

    protected BoxTree(float boxNodeGrowth = 1, Func<int, int>? growthFunc = null, bool locking = false) {
      BoxNodeGrowth = boxNodeGrowth;
      GrowthFunc = growthFunc ?? DefaultGrowthFunc;
      if (locking)
        _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    }

    // box2d grows by *2, here we're being somewhat more linear
    private static int DefaultGrowthFunc(int x)
      => x + 256;

    protected void EnterReadLock() => _lock?.EnterReadLock();

    protected void ExitReadLock() => _lock?.ExitReadLock();

    protected void EnterUpgradeableReadLock() => _lock?.EnterUpgradeableReadLock();

    protected void ExitUpgradeableReadLock() => _lock?.ExitUpgradeableReadLock();

    protected void EnterWriteLock() => _lock?.EnterWriteLock();

    protected void ExitWriteLock() => _lock?.ExitWriteLock();

  }

  [PublicAPI]
  public sealed partial class BoxTree<T>
    : BoxTree, ICollection<T> where T : notnull {

    public delegate BoxF ExtractBoxDelegate(in T value);

    private readonly IEqualityComparer<T> _equalityComparer;

    private readonly ExtractBoxCallsite _extractBox;

    public struct ExtractBoxCallsite {

      private readonly unsafe void* _ptr;

      private readonly object? _tgt;

      public unsafe ExtractBoxCallsite(ExtractBoxDelegate callback) {
        var p = (void***) Unsafe.AsPointer(ref Unsafe.AsRef(callback));
        var methodPtr = (*p)[3];
        var methodPtrAux = (*p)[4];
        var isThisCall = methodPtrAux == default;
        _ptr = isThisCall ? methodPtr : methodPtrAux;
        _tgt = callback.Target;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public unsafe BoxF Invoke(in T value) {
        IL.DeclareLocals(false);
        var tgt = _tgt;
        if (tgt != null) {
          IL.Push(tgt);
          IL.PushInRef(value);
          IL.Push(_ptr);
          IL.Emit.Tail();
          IL.Emit.Calli(new StandAloneMethodSig(CallingConventions.HasThis, typeof(BoxF), typeof(T).MakeByRefType()));
          return IL.Return<BoxF>();
        }

        IL.PushInRef(value);
        IL.Push(_ptr);
        IL.Emit.Tail();
        IL.Emit.Calli(new StandAloneMethodSig(CallingConventions.Standard, typeof(BoxF), typeof(T).MakeByRefType()));
        return IL.Return<BoxF>();
      }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe BoxF ExtractBox(in T value)
      => _extractBox.Invoke(value);

    private Proxy _freeBranches;

    private Proxy _freeLeaves;

    private IDictionary<T, int> _leafLookup;

    private Leaf[] _leaves;

    private Branch[] _branches;

    private Proxy _root;

    private ref Proxy Root {
      get {
        Assert(_root.IsFree || !_root.IsLeaf);
        return ref _root;
      }
    }

    public BoxTree(ExtractBoxDelegate extractBoxFunc, IEqualityComparer<T>? comparer = null, float boxNodeGrowth = 1f / 32, int capacity = 256, Func<int, int>? growthFunc = null, bool locking = false)
      : base(boxNodeGrowth, growthFunc, locking) {
      _extractBox = new ExtractBoxCallsite(extractBoxFunc);
      _equalityComparer = comparer ?? EqualityComparer<T>.Default;
      capacity = Math.Max(MinimumCapacity, capacity);

      Root.Free();

      _leafLookup = new Dictionary<T, int>(capacity);
      _leaves = new Leaf[capacity];
      _branches = new Branch[capacity];

      ResetBranches();
      ResetLeaves();
    }

    private void ResetLeaves() {
      EnterWriteLock();
      try {
        var ll = LeafCapacity - 1;
        for (var i = 0; i < ll; ++i) {
          ref var leaf = ref _leaves[i];
          leaf.Parent = new Proxy(i + 1, true);
          leaf.IsFree = true;
        }

        ref var lastLeaf = ref _leaves[ll];

        lastLeaf.Parent.Free();
        lastLeaf.IsFree = true;

        FreeLeaves = new Proxy(0, true);
        ValidateFreeLeaves();
      }
      finally {
        ExitWriteLock();
      }
    }

    private void ResetBranches() {
      EnterWriteLock();
      try {
        var lb = BranchCapacity - 1;
        for (var i = 0; i < lb; ++i) {
          ref var branch = ref _branches[i];
          branch.Parent = (Proxy) (i + 1);
          branch.IsFree = true;
          branch.Height = 0;
        }

        ref var lastBranch = ref _branches[lb];

        lastBranch.Parent.Free();
        lastBranch.IsFree = true;
        lastBranch.Height = 0;

        FreeBranches = new Proxy(0);
        ValidateFreeBranches();
      }
      finally {
        ExitWriteLock();
      }
    }

    private Proxy FreeBranches {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _freeBranches;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set {
#if DEBUG_DYNAMIC_TREE
        if (!value.IsFree) {
          Assert(!value.IsLeaf);
          Assert(!_branches[(uint)value].IsLeaf);
          Assert(_branches[(uint)value].IsFree);
        }
#endif
        _freeBranches = value;
        ValidateFreeBranches();
      }
    }

    private Proxy FreeLeaves {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _freeLeaves;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set {
        Assert(value.IsLeaf);
#if DEBUG_DYNAMIC_TREE
        if (!value.IsFree) {
          Assert(_leaves[value.LeafIndex].IsLeaf);
          Assert(_leaves[value.LeafIndex].IsFree);
        }
#endif
        _freeLeaves = value;
        ValidateFreeLeaves();
      }
    }

    public void Clear() {
      EnterWriteLock();
      try {
        var branchCapacity = BranchCapacity;
        var leafCapacity = LeafCapacity;

        BranchCount = 0;
        _leaves = new Leaf[leafCapacity];
        _branches = new Branch[branchCapacity];
        _leafLookup = new Dictionary<T, int>(leafCapacity);
        Root.Free();

        ResetBranches();
        ResetLeaves();
      }
      finally {
        ExitWriteLock();
      }
    }

  }

}