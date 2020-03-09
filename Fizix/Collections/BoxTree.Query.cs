using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Fizix {

  public sealed partial class BoxTree<T> {

    public delegate bool QueryCallbackDelegate(ref T value);

    public delegate bool RayQueryCallbackDelegate(ref T value, in Vector2 point, float distFromOrigin);

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public IEnumerable<T> Query(BoxF box, bool approx = false) {
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
            var preciseBox = _extractBox(item);

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
        if(!child1.IsFree)
          stack.Push(child1);

        var child2 = node.Child2;
        if(!child2.IsFree)
          stack.Push(child2);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public IEnumerable<T> Query(Vector2 point, bool approx = false) {
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
            var preciseBox = _extractBox(item);

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

        if(!node.Child1.IsFree)
          stack.Push(node.Child1);

        if(!node.Child2.IsFree)
          stack.Push(node.Child2);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public bool Query(RayQueryCallbackDelegate callback, in Vector2 start, in Vector2 dir, bool approx = false) {
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

            var carryOn = callback(ref item, hit, dist);

            if (!carryOn)
              return true;
          }
          else {
            var preciseBox = _extractBox(item);

            if (!ray.Intersects(preciseBox, out var dist, out var hit))
              continue;

            any = true;

            var carryOn = callback(ref item, hit, dist);

            if (!carryOn)
              return true;
          }

          continue;
        }

        ref var node = ref GetBranch(proxy);

        if (!ray.Intersects(node.Box, out _, out _))
          continue;

        if(!node.Child1.IsFree)
          stack.Push(node.Child1);

        if(!node.Child2.IsFree)
          stack.Push(node.Child2);
      }

      return any;
    }

  }

}