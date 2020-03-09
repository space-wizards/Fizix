using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fizix {

  public sealed partial class BoxTree<T> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<(T A, T B)> GetCollisions(bool approx = false) {
      var stack = new Stack<Proxy>(256);

      ISet<(Proxy, Proxy)> collisions = new HashSet<(Proxy, Proxy)>(_leafLookup.Count);

      for (var i = 0u; i < _leaves.Length; ++i) {
        // skip free leaves
        var leaf = _leaves[i];
        if (leaf.Parent.IsLeaf)
          continue;

        foreach (var pair in GetCollisions(stack, collisions, leaf, new Proxy(i, true), approx))
          yield return (_leaves[pair.A.LeafIndex].Item, _leaves[pair.B.LeafIndex].Item);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    private IEnumerable<(Proxy A, Proxy B)> GetCollisions(Stack<Proxy> stack, ISet<(Proxy, Proxy)> pairs, Leaf leaf, Proxy leafProxy, bool approx = false) {
      stack.Clear();

      var parent = leaf.Parent;

      Assert(!parent.IsFree);

      var box = approx ? leaf.Box : _extractBox(leaf.Item);

      stack.Push(parent);

      while (stack.Count > 0) {
        var proxy = stack.Pop();

        if (proxy.IsFree || proxy == leafProxy)
          continue;

        if (proxy.IsLeaf) {
          var item = _leaves[proxy.LeafIndex].Item;

          if (!approx) {
            var preciseBox = _extractBox(item);

            if (!preciseBox.Intersects(box))
              continue;
          }

          var pair = leafProxy > proxy ? (proxy, leafProxy) : (leafProxy, proxy);

          if (!pairs.Add(pair))
            continue;

          yield return pair;

          continue;
        }

        // note: non-ref stack local copy here
        var node = GetBranch(proxy);

        if (!node.Box.Intersects(box))
          continue;

        if(!node.Child1.IsFree)
          stack.Push(node.Child1);

        if(!node.Child2.IsFree)
          stack.Push(node.Child2);
      }
    }

  }

}