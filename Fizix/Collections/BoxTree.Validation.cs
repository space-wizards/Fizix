using CannyFastMath;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Fizix {

  // ReSharper disable once UnusedTypeParameter
  public sealed partial class BoxTree<T> {

    [Conditional("DEBUG_DYNAMIC_TREE")]
    private void Validate() {
      Validate(Root);
      Assert(BranchCount + ValidateFreeBranches() == BranchCapacity);
      Assert(LeafCount + ValidateFreeLeaves() == LeafCapacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ValidateFreeBranches() {
#if DEBUG_DYNAMIC_TREE
      var freeBranchCount = 0;
      var freeBranchProxy = FreeBranches;
      while (!freeBranchProxy.IsFree) {
        Assert(!freeBranchProxy.IsFree);
        Assert(!freeBranchProxy.IsLeaf);
        Assert((uint) freeBranchProxy < BranchCapacity);
        freeBranchProxy = GetBranch(freeBranchProxy).Parent;
        ++freeBranchCount;
        Assert(freeBranchCount <= BranchCapacity);
      }

      return freeBranchCount;
#else
      return 0;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ValidateFreeLeaves() {
#if DEBUG_DYNAMIC_TREE
      var freeLeafCount = 0;
      var freeLeafProxy = FreeLeaves;
      while (!freeLeafProxy.IsFree) {
        Assert(!freeLeafProxy.IsFree);
        Assert(freeLeafProxy.IsLeaf);
        Assert(freeLeafProxy.LeafIndex < LeafCapacity);
        freeLeafProxy = GetLeaf(freeLeafProxy).Parent;
        ++freeLeafCount;
      }

      return freeLeafCount;
#else
      return 0;
#endif
    }

    [Conditional("DEBUG_DYNAMIC_TREE")]
    private void Validate(Proxy proxy) {
      for (;;) {
        if (proxy.IsFree)
          return;

        if (proxy.IsLeaf)
          return;

        ref var node = ref GetBranch(proxy);
        Assert(!node.IsFree);

        ValidateHeightAndBoxes(node);

        if (proxy == Root)
          Assert(node.Parent.IsFree);

        ref var box = ref node.Box;

        var child1 = node.Child1;
        if (!child1.IsFree) {
          Assert(child1.IsLeaf ? child1.LeafIndex < LeafCapacity : (uint) child1 < BranchCapacity);

          if (child1.IsLeaf) {
            ref var child1Node = ref GetLeaf(child1);
            Assert(!child1Node.IsFree);
            Assert(child1Node.Parent == proxy);
          }
          else {
            ref var child1Node = ref GetBranch(child1);
            Assert(!child1Node.IsFree);
            Assert(child1Node.Parent == proxy);
          }

          ref var child1Box = ref GetBox(child1);
          Assert(box.Contains(child1Box));
          Validate(child1);
        }

        var child2 = node.Child2;
        if (child2.IsFree)
          return;

        Assert(child2.IsLeaf ? child2.LeafIndex < LeafCapacity : (uint) child2 < BranchCapacity);

        if (child2.IsLeaf) {
          ref var child2Node = ref GetLeaf(child2);
          Assert(!child2Node.IsFree);
          Assert(child2Node.Parent == proxy);
        }
        else {
          ref var child2Node = ref GetBranch(child2);
          Assert(!child2Node.IsFree);
          Assert(child2Node.Parent == proxy);
        }

        ref var child2Box = ref GetBox(child2);
        Assert(box.Contains(child2Box));
        proxy = child2;
      }
    }

    [Conditional("DEBUG_DYNAMIC_TREE")]
    [Conditional("DEBUG_DYNAMIC_TREE_ASSERTS")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    private void ValidateHeightAndBoxes(in Branch branch) {
      ref var indexBox = ref Unsafe.AsRef(branch.Box);
      BoxF unionBox;
      var child1 = branch.Child1;
      var child2 = branch.Child2;

      if (child1.IsLeaf) {
        if (child2.IsLeaf) {
          Assert(branch.Height == 2);
          unionBox = child1.IsFree
            ? child2.IsFree
              ? new BoxF()
              : GetBox(child2)
            : child2.IsFree
              ? GetBox(child1)
              : GetBox(child1)
                .Union(GetBox(child2));
        }
        else {
          ref var child1Node = ref GetLeaf(child1);
          ref var child2Node = ref GetBranch(child2);
          Assert(branch.Height == child2Node.Height + 1);
          unionBox = child1Node.Box.Union(child2Node.Box);
        }
      }
      else {
        if (child2.IsLeaf) {
          ref var child1Node = ref GetBranch(child1);
          Assert(branch.Height == child1Node.Height + 1);
          unionBox = child2.IsFree
            ? child1Node.Box
            : child1Node.Box.Union(GetBox(child2));
        }
        else {
          ref var child1Node = ref GetBranch(child1);
          ref var child2Node = ref GetBranch(child2);
          Assert(branch.Height == Math.Max(child1Node.Height, child2Node.Height) + 1);
          unionBox = child1Node.Box.Union(child2Node.Box);
        }
      }

      Assert(indexBox.Equals(unionBox) || indexBox.Contains(unionBox));
    }

  }

}