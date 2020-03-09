using System.Runtime.CompilerServices;

namespace Fizix {

  public sealed partial class BoxTree<T> {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DestroyLeaf(int leafIndex) {
      Validate();
      RemoveLeaf(leafIndex);
      FreeLeaf(leafIndex);
      Validate();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FreeLeaf(int leafIndex) {
      ref var leaf = ref GetLeaf(leafIndex);
      leaf.Parent = FreeLeaves;
      leaf.IsFree = true;
      FreeLeaves = new Proxy(leafIndex, true);
      --LeafCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FreeBranch(Proxy proxy) {
      Assert(!proxy.IsLeaf);
      ref var branch = ref GetBranch(proxy);
      branch.Parent = FreeBranches;
      branch.Child1.Free();
      branch.Child2.Free();
      branch.IsFree = true;
      branch.Height = 0;
      FreeBranches = proxy;
      --BranchCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    private void RemoveLeaf(int leafIndex) {
      ref var leaf = ref _leaves[leafIndex];
      var parent = leaf.Parent;
      var leafProxy = new Proxy((uint) leafIndex, true);
      leaf.Parent.Free();

      if (parent == Root) {
        ref var rootNode = ref GetBranch(parent);
        ref var child1 = ref rootNode.Child1;
        ref var child2 = ref rootNode.Child2;
        if (child1 == leafProxy) {
          if (child2.IsLeaf) {
            if (child2.IsFree) {
              Root.Free();
              FreeBranch(parent);
            }
            else {
              child1.Free();
              rootNode.Height = 2;
            }
          }
          else {
            var newRoot = child2;
            ref var newRootBranch = ref GetBranch(newRoot);
            newRootBranch.Parent.Free();
            Root = newRoot;
            FreeBranch(parent);
          }
        }
        else {
          if (child1.IsLeaf) {
            if (child1.IsFree) {
              Root.Free();
              FreeBranch(parent);
            }
            else {
              child2.Free();
              rootNode.Height = 2;
            }
          }
          else {
            var newRoot = child1;
            ref var newRootBranch = ref GetBranch(newRoot);
            newRootBranch.Parent.Free();
            Root = newRoot;
            FreeBranch(parent);
          }
        }
        return;
      }

      Assert(!parent.IsFree);
      {
        ref var parentBranch = ref GetBranch(parent);
        var grandParent = parentBranch.Parent;
        ref var child1 = ref parentBranch.Child1;
        ref var child2 = ref parentBranch.Child2;
        var sibling = child1 != leafProxy
          ? child1
          : child2;

        if (sibling.IsFree) {
          child1.Free();
          child2.Free();
          FreeBranch(parent);
        }
        else {
          if (sibling.IsLeaf) {
            ref var siblingLeaf = ref GetLeaf(sibling);

            if (grandParent.IsFree) {
              Root.Free();
              siblingLeaf.Parent.Free();
              child1.Free();
              child2.Free();
            }
            else {
              ref var grandParentNode = ref GetBranch(grandParent);
              if (grandParentNode.Child1 == parent)
                grandParentNode.Child1 = sibling;
              else
                grandParentNode.Child2 = sibling;

              siblingLeaf.Parent = grandParent;
              UpdateHeight(grandParent);
            }

            FreeBranch(parent);
          }
          else {
            ref var siblingBranch = ref GetBranch(sibling);

            if (grandParent.IsFree) {
              Root.Free();
              siblingBranch.Parent.Free();
              child1.Free();
              child2.Free();
            }
            else {
              ref var grandParentNode = ref GetBranch(grandParent);
              if (grandParentNode.Child1 == parent)
                grandParentNode.Child1 = sibling;
              else
                grandParentNode.Child2 = sibling;

              siblingBranch.Parent = grandParent;
              //grandParentNode.Height = siblingBranch.Height + 1;
              //UpdateHeight(grandParentNode.Parent);
              UpdateHeight(grandParent);
            }

            FreeBranch(parent);
          }
        }

        //Balance(grandParent);
      }
    }

  }

}