using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fizix {

  public partial class BoxTree {

    [Conditional("DEBUG_DYNAMIC_TREE")]
    [Conditional("DEBUG_DYNAMIC_TREE_ASSERTS")]
    [DebuggerNonUserCode] [DebuggerHidden] [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert(bool assertion, [CallerMemberName] string member = default, [CallerFilePath] string file = default, [CallerLineNumber] int line = default) {
      if (assertion) return;

      var msg = $"Assertion failure in {member} ({file}:{line})";
      Debug.Print(msg);
      Debugger.Break();
      throw new InvalidOperationException(msg);
    }

    protected struct Placeholder : INode {

      public BoxF Box {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
      }

      public Proxy Parent {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new Proxy(int.MaxValue);
      }

      public int Height {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => -1;
      }

      public bool IsFree {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
      }

      public bool IsLeaf {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override string ToString()
        => $@"Placeholder";

    }

  }

  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
  public sealed partial class BoxTree<T> {

    public string DebuggerDisplay
      => $"Count: {Count} ({LeafCount} leaves, {BranchCount} branches), Capacity: ({LeafCapacity} leaves, {BranchCapacity} branches)";

    internal IEnumerable<(Proxy, INode, int)> DebugAllocatedNodesEnumerable {
      [MethodImpl(MethodImplOptions.NoInlining)]
      get {
        for (var i = 0u; i < _branches.Length; i++) {
          var branch = _branches[i];
          yield return ((Proxy) i, branch, CalculateHeight(branch));
        }

        for (var i = 0u; i < _leaves.Length; i++) {
          var leaf = _leaves[i];
          yield return (new Proxy(i, true), leaf, CalculateHeight(leaf));
        }
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    internal (Proxy, INode, int)[] DebugAllocatedNodes {
      [MethodImpl(MethodImplOptions.NoInlining)]
      get {
        var l = _branches.Length + _leaves.Length;
        var data = new (Proxy, INode, int)[l];
        var i = 0;
        foreach (var x in DebugAllocatedNodesEnumerable)
          data[i++] = x;
        while (i > l)
          data[i++] = (new Proxy(uint.MaxValue), default(Placeholder), -1);

        return data;
      }
    }

  }

}