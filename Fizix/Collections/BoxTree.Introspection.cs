using System;
using System.Runtime.CompilerServices;

namespace Fizix {

  public sealed partial class BoxTree<T> {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetProxy(in T item, out Proxy proxy) {
      if (_leafLookup.TryGetValue(item, out var leafIndex)) {
        proxy = new Proxy((uint) leafIndex, true);
        return true;
      }

      proxy = default;
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetProxy(T item, out Proxy proxy) {
      if (_leafLookup.TryGetValue(item, out var leafIndex)) {
        proxy = new Proxy(leafIndex, true);
        return true;
      }

      proxy = default;
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetProxy(in T item, out int leafIndex)
      => _leafLookup.TryGetValue(item, out leafIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoxF? GetBox(T item) {
      EnterReadLock();
      try {
        return TryGetProxy(item, out int leafIndex)
          ? GetLeaf(leafIndex).Box
          : (BoxF?) null;
      }
      finally {
        ExitReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoxF? GetBox(in T item) {
      EnterReadLock();
      try {
        return TryGetProxy(item, out int leafIndex)
          ? GetLeaf(leafIndex).Box
          : (BoxF?) null;
      }
      finally {
        ExitReadLock();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref BoxF GetBox(ref Leaf item)
      => ref Unsafe.As<Leaf, BoxF>(ref item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref BoxF GetBox(ref Branch item)
      => ref Unsafe.As<Branch, BoxF>(ref item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref BoxF GetBox(Proxy proxy) {
      Assert(!proxy.IsFree);

      if (proxy.IsLeaf)
        return ref GetBox(ref GetLeaf(proxy.LeafIndex));

      return ref GetBox(ref GetBranch(proxy));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetParent(Proxy proxy, Proxy newParent) {
      Assert(!proxy.IsFree);

      if (proxy.IsLeaf) {
        ref var leaf = ref GetLeaf(proxy.LeafIndex);
        leaf.Parent = newParent;
        return;
      }

      ref var branch = ref GetBranch(proxy);
      branch.Parent = newParent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref Branch GetBranch(Proxy proxy) {
      Assert(!proxy.IsFree);
      Assert(!proxy.IsLeaf);

      return ref _branches[(int) (uint) proxy];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref Leaf GetLeaf(Proxy proxy) {
      Assert(!proxy.IsFree);
      Assert(proxy.IsLeaf);

      return ref _leaves[proxy.LeafIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref Leaf GetLeaf(int leafIndex) {
      Assert(leafIndex >= 0 && leafIndex <= 0x7FFFFFFEu);
#if DEBUG
      if (leafIndex < 0)
        throw new InvalidOperationException("Not a leaf index.");
#endif

      return ref _leaves[leafIndex];
    }

    private int CalculateHeight(Proxy proxy) {
      if (proxy.IsFree)
        return -1;

      return proxy.IsLeaf
        ? CalculateHeight(GetLeaf(proxy.LeafIndex))
        : CalculateHeight(GetBranch(proxy));
    }


    private int CalculateLeafHeight(in Leaf leaf) {
      if (leaf.IsFree)
        return -1;

      var parent = leaf.Parent;

      if (parent.IsFree)
        return int.MinValue; // detached leaf, could be a NaN

      ref var branch = ref GetBranch(parent);
      
      return 1 + CalculateBranchHeight(branch);
    }

    private int CalculateBranchHeight(in Branch outerBranch) {
      var height = outerBranch.Height;
      if (height > 0)
        return height;
      
      if (outerBranch.IsFree)
        return -1;

      var parent = outerBranch.Parent;
      
      var i = 0;
      do {
        if (i > _branchCount)
          return -i; // a loop must have happened

        Assert(!parent.IsLeaf);

        ref var branch = ref GetBranch(parent);
        
        parent = branch.Parent;

        ++i;
      } while (!parent.IsFree);

      return i;
    }

    private int CalculateHeight(INode node) {
      if (node is Branch br)
        return CalculateBranchHeight(br);
      return CalculateLeafHeight((Leaf)node);
    }

  }

}