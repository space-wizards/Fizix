using System.Runtime.CompilerServices;

namespace Fizix {

  public partial class BoxTree<T> {

    private struct Leaf : INode {

      public BoxF Box;

      BoxF INode.Box {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Box;
      }

      public Proxy Parent;

      Proxy INode.Parent {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Parent;
      }

      public T Item;

      private bool _used;

      public bool IsFree {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !_used;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _used = !value;
      }

      public bool IsLeaf {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
      }

      public override string ToString()
        => $@"Parent: {(Parent.IsFree ? "None" : Parent.ToString())}, {
          (IsFree
            ? "Free"
            : $"Item: {Item}")}";

    }

  }

}