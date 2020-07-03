using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
#if DEBUG
using System.Linq;
#endif

namespace Fizix {

  public sealed partial class BoxTree<T> {

    private int _branchCount;

    private int _leafCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<T>.Add(T item)
      => Add(item);

    public bool Add(in T item) {
      EnterUpgradeableReadLock();
      try {
        if (TryGetProxy(item, out var proxy)) {
          return false;
        }

        EnterWriteLock();
        try {
          var box = _extractBox(item);

          proxy = AllocateLeaf();

          ref var leaf = ref GetLeaf(proxy);
          Assert(!leaf.IsFree);
          leaf.Box = box.Grow(BoxNodeGrowth);
          leaf.Item = item;

          var leafIndex = proxy.LeafIndex;

          InsertLeaf(leafIndex);

          _leafLookup[item] = leafIndex;

          Assert(Contains(item));
#if DEBUG
          // ReSharper disable RedundantAssignment
          var itemCopy = item;
          Assert(this.Any(x => Equals(x, itemCopy)));
          var parent = leaf.Parent;
          if (!box.HasNaN()) {
            Assert(parent == Root || !parent.IsFree);
            Assert(Equals(leaf.Item, item));
          }
          // ReSharper restore RedundantAssignment
#endif
        }
        finally {
          ExitWriteLock();
        }
      }
      finally {
        ExitUpgradeableReadLock();
      }

      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(in T item) {
      EnterWriteLock();
      try {
        if (!_leafLookup.Remove(item, out var leafIndex))
          return false;

        DestroyLeaf(leafIndex);
        return true;
      }
      finally {
        ExitWriteLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<T>.Remove(T item)
      => Remove(item);

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public bool Update(in T item) {
      EnterUpgradeableReadLock();
      try {
        if (!TryGetProxy(item, out int leafIndex))
          return false;

        Assert(Contains(item));
#if DEBUG
        // ReSharper disable RedundantAssignment
        var itemCopy = item;
        Assert(this.Any(x => Equals(x, itemCopy)));
        // ReSharper restore RedundantAssignment
#endif

        ref var leafNode = ref GetLeaf(leafIndex);

        Assert(Equals(leafNode.Item, item));

        var newBox = _extractBox(item);

        if (leafNode.Box.Contains(newBox))
          return false;

        EnterWriteLock();
        try {
          Vector2 movedDist = newBox.Center - leafNode.Box.Center;

          var fattenedNewBox = newBox.Grow(BoxNodeGrowth);

          fattenedNewBox = newBox.Union(fattenedNewBox.Translate(movedDist));

          Assert(fattenedNewBox.Contains(newBox));

          RemoveLeaf(leafIndex);

          leafNode.Box = fattenedNewBox;

          InsertLeaf(leafIndex);

          Assert(Contains(item));
#if DEBUG
          // ReSharper disable RedundantAssignment
          Assert(this.Any(x => Equals(x, itemCopy)));
          var parent = GetLeaf(leafIndex).Parent;
          Assert(parent == Root || !parent.IsFree);
          // ReSharper restore RedundantAssignment
#endif

          return true;
        }
        finally {
          ExitWriteLock();
        }
      }
      finally {
        ExitUpgradeableReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AddOrUpdate(T item) => Update(item) || Add(item);

    public int BranchCapacity {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _branches.Length;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => EnsureBranchCapacity(value);
    }

    public int LeafCapacity {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _leaves.Length;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => EnsureLeafCapacity(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<T> GetEnumerator() {
      EnterReadLock();
      try {
        foreach (var leaf in _leaves) {
          if (leaf.IsFree) continue;

          yield return leaf.Item;
        }
      }
      finally {
        ExitReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    public void CopyTo(T[] array, int arrayIndex) {
      EnterReadLock();
      try {
        _leafLookup.Keys.CopyTo(array, arrayIndex);
      }
      finally {
        ExitReadLock();
      }
    }

    public int BranchCount {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _branchCount;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private set {
        Assert(_branchCount >= 0);
        Assert(value >= 0);
        _branchCount = value;
      }
    }

    public int LeafCount {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _leafCount;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private set {
        Assert(_leafCount >= 0);
        Assert(value >= 0);
        _leafCount = value;
      }
    }

    public int Count => _leafLookup.Count;

    public bool IsReadOnly
      => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(T item)
      => item != null && _leafLookup.ContainsKey(item);

  }

}