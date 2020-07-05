using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using InlineIL;
using JetBrains.Annotations;

namespace Fizix {

  public sealed partial class BoxTree<T> {

    public delegate bool QueryCallbackDelegate(ref T value);

    public struct QueryCallbackCallsite {

      private readonly unsafe void* _ptr;

      private readonly object? _tgt;

      public unsafe QueryCallbackCallsite(QueryCallbackDelegate callback) {
        var p = (void***) Unsafe.AsPointer(ref Unsafe.AsRef(callback));
        var methodPtr = (*p)[3];
        var methodPtrAux = (*p)[4];
        var isThisCall = methodPtrAux == default;
        _ptr = isThisCall ? methodPtr : methodPtrAux;
        _tgt = callback.Target;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public unsafe bool Invoke(in T value) {
        IL.DeclareLocals(false);
        var tgt = _tgt;
        if (tgt != null) {
          IL.Push(tgt);
          IL.PushInRef(value);
          IL.Push(_ptr);
          IL.Emit.Tail();
          IL.Emit.Calli(new StandAloneMethodSig(CallingConventions.HasThis, typeof(bool), typeof(T).MakeByRefType()));
          return IL.Return<bool>();
        }

        IL.PushInRef(value);
        IL.Push(_ptr);
        IL.Emit.Tail();
        IL.Emit.Calli(new StandAloneMethodSig(CallingConventions.Standard, typeof(bool), typeof(T).MakeByRefType()));
        return IL.Return<bool>();
      }

    }
    public delegate bool RayQueryCallbackDelegate(ref T value, Vector2 point, float distFromOrigin);

    public struct RayQueryCallbackCallsite {

      private readonly unsafe void* _ptr;

      private readonly object? _tgt;

      public unsafe RayQueryCallbackCallsite(RayQueryCallbackDelegate callback) {
        var p = (void***) Unsafe.AsPointer(ref Unsafe.AsRef(callback));
        var methodPtr = (*p)[3];
        var methodPtrAux = (*p)[4];
        var isThisCall = methodPtrAux == default;
        _ptr = isThisCall ? methodPtr : methodPtrAux;
        _tgt = callback.Target;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public unsafe bool Invoke(ref T value, Vector2 point, float distFromOrigin) {
        IL.DeclareLocals(false);
        var tgt = _tgt;
        if (tgt != null) {
          IL.Push(tgt);
          IL.PushInRef(value);
          IL.Push(_ptr);
          IL.Emit.Tail();
          IL.Emit.Calli(new StandAloneMethodSig(CallingConventions.HasThis, typeof(bool), typeof(T).MakeByRefType(), typeof(Vector2), typeof(float)));
          return IL.Return<bool>();
        }

        IL.PushInRef(value);
        IL.Push(_ptr);
        IL.Emit.Tail();
        IL.Emit.Calli(new StandAloneMethodSig(CallingConventions.Standard, typeof(bool), typeof(T).MakeByRefType(), typeof(Vector2), typeof(float)));
        return IL.Return<bool>();
      }

    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public IEnumerable<T> Query(BoxF box, bool approx = false) {
      EnterReadLock();
      try {
        var stack = new Stack<Proxy>(256);

        stack.Push(Root);

        while (stack.Count > 0) {
          var proxy = stack.Pop();

          if (proxy.IsFree)
            continue;

          if (proxy.IsLeaf) {
            // note: non-ref stack local copy here
            var leaf = GetLeaf(proxy);

            var nodeBox = leaf.Box;

            if (!nodeBox.Intersects(box))
              continue;

            var item = leaf.Item;

            if (!approx) {
              var preciseBox = ExtractBox(item);

              if (!preciseBox.Intersects(box))
                continue;
            }

            yield return item;

            continue;
          }

          var node = GetBranch(proxy);

          if (!node.Box.Intersects(box))
            continue;

          var child1 = node.Child1;
          if (!child1.IsFree)
            stack.Push(child1);

          var child2 = node.Child2;
          if (!child2.IsFree)
            stack.Push(child2);
        }
      }
      finally {
        ExitReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public void Query([InstantHandle]QueryCallbackDelegate callback, BoxF box, bool approx = false) {
      var cb = new QueryCallbackCallsite(callback);
      EnterReadLock();
      try {
        var stack = new Stack<Proxy>(256);

        stack.Push(Root);

        while (stack.Count > 0) {
          var proxy = stack.Pop();

          if (proxy.IsFree)
            continue;

          if (proxy.IsLeaf) {
            // note: non-ref stack local copy here
            var leaf = GetLeaf(proxy);

            var nodeBox = leaf.Box;

            if (!nodeBox.Intersects(box))
              continue;

            var item = leaf.Item;

            if (!approx) {
              var preciseBox = ExtractBox(item);

              if (!preciseBox.Intersects(box))
                continue;
            }

            //yield return item;
            if (!cb.Invoke(item))
              return;

            continue;
          }

          var node = GetBranch(proxy);

          if (!node.Box.Intersects(box))
            continue;

          var child1 = node.Child1;
          if (!child1.IsFree)
            stack.Push(child1);

          var child2 = node.Child2;
          if (!child2.IsFree)
            stack.Push(child2);
        }
      }
      finally {
        ExitReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public IEnumerable<T> Query(Vector2 point, bool approx = false) {
      EnterReadLock();
      try {
        var stack = new Stack<Proxy>(256);

        stack.Push(Root);

        while (stack.Count > 0) {
          var proxy = stack.Pop();

          if (proxy.IsFree)
            continue;

          if (proxy.IsLeaf) {
            var leaf = GetLeaf(proxy);

            var nodeBox = leaf.Box;

            if (!nodeBox.Contains(point))
              continue;

            var item = leaf.Item;

            if (!approx) {
              var preciseBox = ExtractBox(item);

              if (!preciseBox.Contains(point))
                continue;
            }

            yield return item;

            continue;
          }

          // note: non-ref stack local copy here
          var node = GetBranch(proxy);

          if (!node.Box.Contains(point))
            continue;

          if (!node.Child1.IsFree)
            stack.Push(node.Child1);

          if (!node.Child2.IsFree)
            stack.Push(node.Child2);
        }
      }
      finally {
        ExitReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public void Query([InstantHandle]QueryCallbackDelegate callback, Vector2 point, bool approx = false) {
      var cb = new QueryCallbackCallsite(callback);
      EnterReadLock();
      try {
        var stack = new Stack<Proxy>(256);

        stack.Push(Root);

        while (stack.Count > 0) {
          var proxy = stack.Pop();

          if (proxy.IsFree)
            continue;

          if (proxy.IsLeaf) {
            var leaf = GetLeaf(proxy);

            var nodeBox = leaf.Box;

            if (!nodeBox.Contains(point))
              continue;

            var item = leaf.Item;

            if (!approx) {
              var preciseBox = ExtractBox(item);

              if (!preciseBox.Contains(point))
                continue;
            }

            //yield return item;
            if (!cb.Invoke(item))
              return;

            continue;
          }

          // note: non-ref stack local copy here
          var node = GetBranch(proxy);

          if (!node.Box.Contains(point))
            continue;

          if (!node.Child1.IsFree)
            stack.Push(node.Child1);

          if (!node.Child2.IsFree)
            stack.Push(node.Child2);
        }
      }
      finally {
        ExitReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public bool Query([InstantHandle]RayQueryCallbackDelegate callback, Vector2 start, Vector2 dir, bool approx = false) {
      var cb = new RayQueryCallbackCallsite(callback);
      EnterReadLock();
      try {
        var stack = new Stack<Proxy>(256);

        stack.Push(Root);

        var any = false;

        var ray = new RayF(start, dir);

        while (stack.Count > 0) {
          var proxy = stack.Pop();

          if (proxy.IsFree)
            continue;

          if (proxy.IsLeaf) {
            // the ray should always have intersected the box for the branch

            ref var leaf = ref GetLeaf(proxy);

            ref var item = ref leaf.Item;

            if (approx) {
              if (!ray.Intersects(leaf.Box, out var dist, out var hit))
                continue;

              any = true;

              if (!cb.Invoke(ref item, hit, dist))
                return true;
            }
            else {
              var preciseBox = ExtractBox(item);

              if (!ray.Intersects(preciseBox, out var dist, out var hit))
                continue;

              any = true;

              if (!cb.Invoke(ref item, hit, dist))
                return true;
            }

            continue;
          }

          ref var node = ref GetBranch(proxy);

          if (!ray.Intersects(node.Box, out _, out _))
            continue;

          if (!node.Child1.IsFree)
            stack.Push(node.Child1);

          if (!node.Child2.IsFree)
            stack.Push(node.Child2);
        }

        return any;
      }
      finally {
        ExitReadLock();
      }
    }

  }

}