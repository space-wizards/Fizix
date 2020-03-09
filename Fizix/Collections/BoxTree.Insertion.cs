using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using CannyFastMath;
using Math = CannyFastMath.Math;

namespace Fizix {

  public sealed partial class BoxTree<T> {

    /// <remarks>
    ///     If allocation occurs, references to leaves will be invalid.
    /// </remarks>
    private Proxy AllocateLeaf() {
      var alloc = FreeLeaves;

      if (alloc.IsFree) {
        var newLeafCap = GrowthFunc(LeafCapacity);

        if (newLeafCap <= LeafCapacity)
          throw new InvalidOperationException("Growth function returned invalid new capacity, must be greater than current capacity.");

        EnsureLeafCapacity(newLeafCap);

        alloc = FreeLeaves;
        Assert(!alloc.IsFree);
      }

      Assert(alloc.IsLeaf);

      ref var leaf = ref GetLeaf(alloc.LeafIndex);
      Assert(leaf.IsFree);
      FreeLeaves = leaf.Parent;
      leaf.Parent.Free();
      leaf.IsFree = false;
      ++LeafCount;
      return alloc;
    }

    /// <remarks>
    ///     If allocation occurs, references to nodes will be invalid.
    /// </remarks>
    private Proxy AllocateBranch() {
      var alloc = FreeBranches;

      if (alloc.IsFree) {
        var newBranchCap = GrowthFunc(BranchCapacity);

        if (newBranchCap <= BranchCapacity)
          throw new InvalidOperationException("Growth function returned invalid new capacity, must be greater than current capacity.");

        EnsureBranchCapacity(newBranchCap);

        alloc = FreeBranches;
        Assert(!alloc.IsFree);
      }

      Assert(!alloc.IsLeaf);

      ref var branch = ref GetBranch(alloc);
      Assert(branch.IsFree);
      FreeBranches = branch.Parent;
      Assert(FreeBranches.IsFree || GetBranch(FreeBranches).IsFree);
      branch.Parent.Free();
      branch.Child1.Free();
      branch.Child2.Free();
      branch.IsFree = false;
      ++BranchCount;
      return alloc;
    }

    public void EnsureBranchCapacity(int newCapacity) {
      if (newCapacity <= BranchCapacity)
        return;

      var oldNodes = _branches;

      _branches = new Branch[newCapacity];

      Array.Copy(oldNodes, _branches, BranchCount);

      var lb = BranchCapacity - 1;
      for (var i = BranchCount; i < lb; ++i) {
        ref var branch = ref _branches[i];
        branch.Parent = (Proxy) (i + 1);
        branch.IsFree = true;
      }

      ref var lastBranch = ref _branches[lb];
      lastBranch.Parent.Free();
      lastBranch.IsFree = true;
      FreeBranches = (Proxy) BranchCount;
    }

    public void EnsureLeafCapacity(int newCapacity) {
      if (newCapacity <= LeafCapacity)
        return;

      var oldLeaves = _leaves;

      _leaves = new Leaf[newCapacity];

      Array.Copy(oldLeaves, _leaves, LeafCount);

      var ll = LeafCapacity - 1;
      for (var i = (uint) LeafCount; i < ll; ++i) {
        ref var leaf = ref _leaves[i];
        leaf.Parent = new Proxy(i + 1, true);
        leaf.IsFree = true;
      }

      ref var lastLeaf = ref _leaves[ll];
      lastLeaf.Parent.Free();
      lastLeaf.IsFree = true;
      FreeLeaves = new Proxy(LeafCount, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    private void InsertLeaf(int leafIndex) {
      Validate();

      var leafProxy = new Proxy(leafIndex, true);

      ref var leaf = ref _leaves[leafIndex];

      if (leaf.Box.HasNaN())
        return;

      Assert(!leaf.IsFree);

      if (Root.IsFree) {
        var alloc = AllocateBranch();
        Assert(!alloc.IsFree);
        Root = alloc;
        ref var root = ref GetBranch(Root);
        root.Height = 2;
        root.Box = leaf.Box.Grow(BoxNodeGrowth);
        leaf.Parent = alloc;
        root.Child1 = leafProxy;
        Assert(Unsafe.AreSame(ref GetLeaf(root.Child1), ref leaf));
        Validate();
        return;
      }

      _leafLookup[leaf.Item] = leafIndex;

      ref var leafBox = ref leaf.Box;

      var descendent = Root;

      Assert(!descendent.IsLeaf);

#if DEBUG
      var loopCount = 0;
#endif
      var haveCombinedBox = false;
      BoxF combinedBox = default;
      for (;;) {
#if DEBUG
        Assert(loopCount++ < BranchCapacity * 2);
#endif

        if (descendent.IsLeaf) break;

        ref var descendentBranch = ref GetBranch(descendent);

        // assert no loops
        var child1 = descendentBranch.Child1;
        var child2 = descendentBranch.Child2;

#if DEBUG
        if (!child1.IsLeaf) {
          Assert(GetBranch(child1).Parent == descendent);
          Assert(GetBranch(child1).Child1 != descendent);
          Assert(GetBranch(child1).Child2 != descendent);
        }

        if (!child2.IsLeaf) {
          Assert(GetBranch(child2).Parent == descendent);
          Assert(GetBranch(child2).Child1 != descendent);
          Assert(GetBranch(child2).Child2 != descendent);
        }
#endif

        ref var descendentBox = ref descendentBranch.Box;
        var indexPeri = descendentBox.Perimeter();
        Assert(indexPeri >= 0);
        if (!haveCombinedBox)
          combinedBox = descendentBox.Union(leafBox);
        else
          Assert(combinedBox.Equals(descendentBox.Union(leafBox)));
        var combinedPeri = combinedBox.Perimeter();
        Assert(!float.IsNaN(combinedPeri));
        Assert(combinedPeri >= 0);
        var cost = 2 * combinedPeri;
        var inheritCost = 2 * (combinedPeri - indexPeri);

        var cost1 = inheritCost;
        var cost2 = inheritCost;

        BoxF unionBox1;
        if (child1.IsFree) {
          // ReSharper disable once CompareOfFloatsByEqualityOperator
          if (inheritCost == 0f && combinedBox.Equals(descendentBox)) {
            Assert(!descendent.IsFree);
            leaf.Parent = descendent;
            descendentBranch.Child1 = new Proxy(leafIndex, true);
            Assert(Unsafe.AreSame(ref GetLeaf(descendentBranch.Child1), ref leaf));
            if (!descendentBox.Contains(leafBox)) {
              descendentBranch.Box = combinedBox;
              if (!descendentBranch.Parent.IsFree)
                UpdateHeightAndBoxes(descendentBranch.Parent, combinedBox);
            }

            Validate();
            return;
          }

          unionBox1 = combinedBox;
        }
        else {
          ref var child1Box = ref GetBox(child1);
          cost1 += EstimateInsertionCost(leafBox, child1Box, out unionBox1);
        }

        BoxF unionBox2;
        if (child2.IsFree) {
          // ReSharper disable once CompareOfFloatsByEqualityOperator
          if (inheritCost == 0f) {
            Assert(!descendent.IsFree);
            leaf.Parent = descendent;
            descendentBranch.Child2 = new Proxy(leafIndex, true);
            Assert(Unsafe.AreSame(ref GetLeaf(descendentBranch.Child2), ref leaf));
            if (!descendentBox.Contains(leafBox)) {
              descendentBranch.Box = combinedBox;
              if (!descendentBranch.Parent.IsFree)
                UpdateHeightAndBoxes(descendentBranch.Parent, combinedBox);
            }

            Validate();
            return;
          }

          unionBox2 = combinedBox;
        }
        else {
          ref var child2Box = ref GetBox(child2);
          cost2 += EstimateInsertionCost(leafBox, child2Box, out unionBox2);
        }

        if (cost < cost1 && cost < cost2)
          break;

        var pickChild1 = cost1 < cost2;
        var nextRel = pickChild1 ? child1 : child2;
        combinedBox = pickChild1 ? unionBox1 : unionBox2;
        haveCombinedBox = true;

        Assert(!descendent.IsFree);

        if (nextRel.IsFree) {
          if (pickChild1) {
            leaf.Parent = descendent;
            descendentBranch.Box = unionBox1;
            descendentBranch.Child1 = new Proxy(leafIndex, true);
            Assert(Unsafe.AreSame(ref GetLeaf(descendentBranch.Child1), ref leaf));
            Validate();
            return;
          }
          else {
            leaf.Parent = descendent;
            descendentBranch.Box = unionBox2;
            descendentBranch.Child2 = new Proxy(leafIndex, true);
            Assert(Unsafe.AreSame(ref GetLeaf(descendentBranch.Child2), ref leaf));
            Validate();
            return;
          }
        }

        descendent = nextRel;
        Assert(!descendent.IsFree);
      }

      Validate();
      var sibling = descendent;

      Proxy newParent;
      if (sibling.IsLeaf) {
        newParent = AllocateBranch();
        ref var newParentBranch = ref GetBranch(newParent);
        newParentBranch.Height = 2;

        ref var siblingLeaf = ref GetLeaf(sibling);

        var oldParent = siblingLeaf.Parent;

        newParentBranch.Parent = oldParent;
        newParentBranch.Box = leafBox.Union(siblingLeaf.Box);

        if (oldParent.IsFree) {
          newParentBranch.Child1 = sibling;
          newParentBranch.Child2 = leafProxy;
          siblingLeaf.Parent = newParent;
          leaf.Parent = newParent;
          Assert(Unsafe.AreSame(ref GetLeaf(newParentBranch.Child1), ref siblingLeaf));
          Assert(Unsafe.AreSame(ref GetLeaf(newParentBranch.Child2), ref leaf));
          Root = newParent;
          Validate();
          ValidateHeightAndBoxes(GetBranch(leaf.Parent));
        }
        else {
          ref var oldParentBranch = ref GetBranch(oldParent);
          if (oldParentBranch.Child1 == sibling) {
            oldParentBranch.Child1 = newParent;
            Assert(Unsafe.AreSame(ref GetBranch(oldParentBranch.Child1), ref newParentBranch));
          }
          else {
            oldParentBranch.Child2 = newParent;
            Assert(Unsafe.AreSame(ref GetBranch(oldParentBranch.Child2), ref newParentBranch));
          }

          newParentBranch.Child1 = sibling;
          newParentBranch.Child2 = leafProxy;
          siblingLeaf.Parent = newParent;
          leaf.Parent = newParent;
          Assert(Unsafe.AreSame(ref GetLeaf(newParentBranch.Child1), ref siblingLeaf));
          Assert(Unsafe.AreSame(ref GetLeaf(newParentBranch.Child2), ref leaf));

          UpdateHeightAndBoxes(oldParent, oldParentBranch.Box.Union(newParentBranch.Box));
          Validate();
          ValidateHeightAndBoxes(GetBranch(leaf.Parent));
        }
      }
      else {
        newParent = AllocateBranch();
        ref var newParentBranch = ref GetBranch(newParent);

        ref var siblingBranch = ref GetBranch(sibling);

        var newParentHeight = 1 + siblingBranch.Height;
        newParentBranch.Height = newParentHeight;

        var oldParent = siblingBranch.Parent;

        newParentBranch.Parent = oldParent;
        newParentBranch.Box = leafBox.Union(siblingBranch.Box);

        if (oldParent.IsFree) {
          newParentBranch.Child1 = sibling;
          newParentBranch.Child2 = leafProxy;

          siblingBranch.Parent = newParent;
          leaf.Parent = newParent;
          Assert(Unsafe.AreSame(ref GetBranch(newParentBranch.Child1), ref siblingBranch));
          Assert(Unsafe.AreSame(ref GetLeaf(newParentBranch.Child2), ref leaf));
          Root = newParent;
          Validate();
          ValidateHeightAndBoxes(GetBranch(leaf.Parent));
        }
        else {
          ref var oldParentBranch = ref GetBranch(oldParent);

          if (oldParentBranch.Child1 == sibling) {
            oldParentBranch.Child1 = newParent;
            Assert(Unsafe.AreSame(ref GetBranch(oldParentBranch.Child1), ref newParentBranch));
          }
          else {
            oldParentBranch.Child2 = newParent;
            Assert(Unsafe.AreSame(ref GetBranch(oldParentBranch.Child2), ref newParentBranch));
          }

          newParentBranch.Child1 = sibling;
          newParentBranch.Child2 = leafProxy;

          siblingBranch.Parent = newParent;
          leaf.Parent = newParent;
          Assert(Unsafe.AreSame(ref GetBranch(newParentBranch.Child1), ref siblingBranch));
          Assert(Unsafe.AreSame(ref GetLeaf(newParentBranch.Child2), ref leaf));

          UpdateHeightAndBoxes(oldParent, oldParentBranch.Box.Union(newParentBranch.Box));
          Validate();
          ValidateHeightAndBoxes(GetBranch(leaf.Parent));
        }
      }

      Assert(!newParent.IsFree);

      Assert(!leaf.Parent.IsFree);

      Balance(leaf.Parent);

      Assert(!leaf.Parent.IsFree);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Balance(Proxy index) {
      {
        ValidateHeightAndBoxes(GetBranch(index));
      }

#if DEBUG_DYNAMIC_TREE
      var initStep = index;
#endif

      while (!index.IsFree) {
        if (index.IsLeaf) {
          index = GetLeaf(index).Parent;
          continue;
        }

        ValidateHeightAndBoxes(GetBranch(index));
        Validate();
        
        index = BalanceStep(index);

        ref var indexNode = ref GetBranch(index);
        ValidateHeightAndBoxes(indexNode);
        Validate();

#if DEBUG_DYNAMIC_TREE
        var prevStep = index;
#endif
        index = indexNode.Parent;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private Proxy BalanceStep(Proxy iA) {
      ref var a = ref GetBranch(iA);

      if (a.IsLeaf || a.Height < 2)
        return iA;

      var iB = a.Child1;
      var iC = a.Child2;
      Assert(iA != iB);
      Assert(iA != iC);
      Assert(iB != iC);

      // don't balance nodes with leaves
      if (iB.IsLeaf || iC.IsLeaf)
        return iA;

      ref var b = ref GetBranch(iB);
      ref var c = ref GetBranch(iC);

      var balance = c.Height - b.Height;

      // Rotate C up
      if (balance > 1) {
        var iF = c.Child1;
        var iG = c.Child2;
        Assert(iC != iF);
        Assert(iC != iG);
        Assert(iF != iG);

        // don't balance nodes with leaves
        if (iF.IsLeaf || iG.IsLeaf)
          return iA;

        ref var f = ref GetBranch(iF);
        ref var g = ref GetBranch(iG);

        // A <> C

        // this creates a loop ...
        c.Child1 = iA;
        c.Parent = a.Parent;
        a.Parent = iC;

        if (c.Parent.IsFree)
          _root = iC;
        else {
          ref var cParent = ref GetBranch(c.Parent);
          if (cParent.Child1 == iA)
            cParent.Child1 = iC;
          else {
            Assert(cParent.Child2 == iA);
            cParent.Child2 = iC;
          }
        }

        // Rotate
        if (f.Height > g.Height) {
          c.Child2 = iF;
          a.Child2 = iG;
          g.Parent = iA;

          a.Box = b.Box.Union(g.Box);
          c.Box = a.Box.Union(f.Box);

          a.Height = Math.Max(b.Height, g.Height) + 1;
          c.Height = Math.Max(a.Height, f.Height) + 1;
        }
        else {
          c.Child2 = iG;
          a.Child2 = iF;
          f.Parent = iA;
          a.Box = b.Box.Union(f.Box);
          c.Box = a.Box.Union(g.Box);

          a.Height = Math.Max(b.Height, f.Height) + 1;
          c.Height = Math.Max(a.Height, g.Height) + 1;
        }

        if (c.Parent.IsFree)
          return iC;

        UpdateHeightAndBoxes(a.Parent, a.Box);
        UpdateHeightAndBoxes(c.Parent, c.Box);

        return iC;
      }

      // Rotate B up
      if (balance < -1) {
        var iD = b.Child1;
        var iE = b.Child2;
        Assert(iB != iD);
        Assert(iB != iE);
        Assert(iD != iE);

        // don't balance nodes with leaves
        if (iD.IsLeaf || iE.IsLeaf)
          return iA;

        ref var d = ref GetBranch(iD);
        ref var e = ref GetBranch(iE);

        // A <> B

        // this creates a loop ...
        b.Child1 = iA;
        b.Parent = a.Parent;
        a.Parent = iB;

        if (b.Parent.IsFree)
          _root = iB;
        else {
          ref var bParent = ref GetBranch(b.Parent);
          if (bParent.Child1 == iA)
            bParent.Child1 = iB;
          else {
            Assert(bParent.Child2 == iA);
            bParent.Child2 = iB;
          }
        }

        // Rotate
        if (d.Height > e.Height) {
          b.Child2 = iD;
          a.Child1 = iE;
          e.Parent = iA;

          a.Box = c.Box.Union(e.Box);
          b.Box = a.Box.Union(d.Box);

          a.Height = Math.Max(c.Height, e.Height) + 1;
          b.Height = Math.Max(a.Height, d.Height) + 1;
        }
        else {
          b.Child2 = iE;
          a.Child1 = iD;
          d.Parent = iA;

          UpdateHeightAndBoxes(iB, a.Box.Union(e.Box));
          UpdateHeightAndBoxes(iA, c.Box.Union(d.Box));

          Assert(a.Height == Math.Max(c.Height, d.Height) + 1);
          Assert(b.Height == Math.Max(a.Height, e.Height) + 1);
        }

        if (b.Parent.IsFree)
          return iB;

        UpdateHeightAndBoxes(a.Parent, a.Box);
        UpdateHeightAndBoxes(b.Parent, b.Box);

        return iB;
      }

      ValidateHeightAndBoxes(a);

      return iA;
    }

    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void UpdateHeight(Proxy proxy) {
      if (proxy.IsFree)
        return;
      ref var branch = ref GetBranch(proxy);
      var branchHeight = 2;
      if (branch.Child1.IsFree) {
        if (!branch.Child2.IsFree)
          if (!branch.Child2.IsLeaf)
            branchHeight = GetBranch(branch.Child2).Height + 1;
      }
      else if (branch.Child2.IsFree) {
        if (!branch.Child1.IsFree)
          if (!branch.Child1.IsLeaf)
            branchHeight = GetBranch(branch.Child1).Height + 1;
      }
      else {
        if (!branch.Child1.IsLeaf) {
          if (!branch.Child2.IsLeaf)
            branchHeight = Math.Max(GetBranch(branch.Child1).Height, GetBranch(branch.Child2).Height) + 1;
          else
            branchHeight = GetBranch(branch.Child1).Height + 1;
        }
        else if (!branch.Child2.IsLeaf)
          branchHeight = GetBranch(branch.Child2).Height + 1;
      }

      branch.Height = branchHeight;
      ValidateHeightAndBoxes(branch);
      var parent = branch.Parent;
      for (;;) {
        if (parent.IsFree)
          break;

        ref var parentBranch = ref GetBranch(parent);
        bool useSiblingHeight;
        var whichSibling = -1;
        if (parentBranch.Child1 == proxy) {
          useSiblingHeight = !parentBranch.Child2.IsLeaf;
          whichSibling = 1;
        }
        else {
          Assert(parentBranch.Child2 == proxy);
          useSiblingHeight = !parentBranch.Child1.IsLeaf;
          whichSibling = 0;
        }

        var sibling = whichSibling == 0 ? parentBranch.Child1 : parentBranch.Child2;
        if (useSiblingHeight) {
          ref var siblingBranch = ref GetBranch(sibling);
          Assert(!Unsafe.AreSame(ref siblingBranch, ref parentBranch));
          var siblingHeight = siblingBranch.Height;

          if (siblingHeight > branchHeight)
            branchHeight = siblingHeight;

        }

        Assert(parentBranch.Box.Contains(useSiblingHeight ? ref GetBranch(sibling).Box : ref GetLeaf(sibling).Box));

        branchHeight += 1;
        if (parentBranch.Height == branchHeight) {
          ValidateHeightAndBoxes(parentBranch);
#if DEBUG_DYNAMIC_TREE          
          if (!parentBranch.Parent.IsFree)
            ValidateHeightAndBoxes(GetBranch(parentBranch.Parent));
#endif
          break;
        }

        parentBranch.Height = branchHeight;
        proxy = parent;
        parent = parentBranch.Parent;
        branchHeight = parentBranch.Height;
        ValidateHeightAndBoxes(parentBranch);
      }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private unsafe void UpdateHeightAndBoxes(Proxy proxy, in BoxF box) {
      ref var branch = ref GetBranch(proxy);
      var branchHeight = 2;
      if (branch.Child1.IsFree) {
        if (!branch.Child2.IsFree)
          if (!branch.Child2.IsLeaf)
            branchHeight = GetBranch(branch.Child2).Height + 1;
      }
      else if (branch.Child2.IsFree) {
        if (!branch.Child1.IsFree)
          if (!branch.Child1.IsLeaf)
            branchHeight = GetBranch(branch.Child1).Height + 1;
      }
      else {
        if (!branch.Child1.IsLeaf) {
          if (!branch.Child2.IsLeaf)
            branchHeight = Math.Max(GetBranch(branch.Child1).Height, GetBranch(branch.Child2).Height) + 1;
          else
            branchHeight = GetBranch(branch.Child1).Height + 1;
        }
        else if (!branch.Child2.IsLeaf)
          branchHeight = GetBranch(branch.Child2).Height + 1;
      }

      branch.Height = branchHeight;
      UpdateBox(ref branch, box);
      var updateBox = true;
      ValidateHeightAndBoxes(branch);
      var prevBoxPtr = (BoxF*) Unsafe.AsPointer(ref Unsafe.AsRef(box));
      var parent = branch.Parent;
      for (;;) {
        if (parent.IsFree)
          break;

        ref var prevBox = ref Unsafe.AsRef<BoxF>(prevBoxPtr);

        ref var parentBranch = ref GetBranch(parent);
        var useSiblingHeight = false;
        var whichSibling = -1;
        if (parentBranch.Child1 == proxy) {
          useSiblingHeight = !parentBranch.Child2.IsLeaf;
          whichSibling = 1;
        }
        else {
          Assert(parentBranch.Child2 == proxy);
          useSiblingHeight = !parentBranch.Child1.IsLeaf;
          whichSibling = 0;
        }

        var sibling = whichSibling == 0 ? parentBranch.Child1 : parentBranch.Child2;
        if (useSiblingHeight) {
          ref var siblingBranch = ref GetBranch(sibling);
          Assert(!Unsafe.AreSame(ref siblingBranch, ref parentBranch));
          var siblingHeight = siblingBranch.Height;

          if (siblingHeight > branchHeight)
            branchHeight = siblingHeight;

          if (updateBox)
            updateBox = UpdateBox(ref parentBranch, prevBox, siblingBranch.Box);
        }
        else {
          if (updateBox)
            updateBox = UpdateBox(ref parentBranch, prevBox, GetLeaf(sibling).Box);
        }

        prevBoxPtr = (BoxF*) Unsafe.AsPointer(ref parentBranch.Box);

        Assert(parentBranch.Box.Contains(prevBox));
        Assert(parentBranch.Box.Contains(useSiblingHeight ? ref GetBranch(sibling).Box : ref GetLeaf(sibling).Box));

        branchHeight += 1;
        if (parentBranch.Height == branchHeight) {
          ValidateHeightAndBoxes(parentBranch);
          
          if (updateBox && !parentBranch.Parent.IsFree) {
            UpdateBoxes(ref GetBranch(parentBranch.Parent), prevBox);
            ValidateHeightAndBoxes(GetBranch(parentBranch.Parent));
          }
          break;
        }

        parentBranch.Height = branchHeight;
        proxy = parent;
        parent = parentBranch.Parent;
        branchHeight = parentBranch.Height;
        ValidateHeightAndBoxes(parentBranch);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool UpdateBox(ref Branch branch, in BoxF box1, in BoxF box2)
      => UpdateBox(ref branch, box1)
        | UpdateBox(ref branch, box2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool UpdateBox(ref Branch branch, in BoxF newBox) {
      if (branch.Box.Contains(newBox))
        return false;

      branch.Box = branch.Box.Union(newBox);
      Assert(branch.Box.Contains(newBox));
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private unsafe void UpdateBoxes(ref Branch firstBranch, in BoxF newBox) {
      if (!UpdateBox(ref firstBranch, newBox)) {
        Assert(firstBranch.Box.Contains(newBox));
        return;
      }

      var proxy = firstBranch.Parent;
      if (proxy.IsFree)
        return;

      var prevBoxPtr = (BoxF*) Unsafe.AsPointer(ref firstBranch.Box);
      do {
        ref var prevBox = ref Unsafe.AsRef<BoxF>(prevBoxPtr);
        ref var branch = ref GetBranch(proxy);

        if (!UpdateBox(ref branch, prevBox)) {
          Assert(branch.Box.Contains(prevBox));
          return;
        }

        Assert(branch.Box.Contains(prevBox));

        prevBoxPtr = (BoxF*) Unsafe.AsPointer(ref branch.Box);
        proxy = branch.Parent;
      } while (!proxy.IsFree);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float EstimateInsertionCost(in BoxF firstBox, in BoxF addedBox, out BoxF unionBox) {
      unionBox = firstBox.Union(addedBox);
      var cost = unionBox.Perimeter() - addedBox.Perimeter();
      return cost;
    }

  }

}