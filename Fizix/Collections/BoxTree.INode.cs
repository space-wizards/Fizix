using System.Runtime.CompilerServices;

namespace Fizix {

  public partial class BoxTree {

    internal interface INode {

      BoxF Box {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
      }

      Proxy Parent {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
      }

      bool IsFree {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
      }

      bool IsLeaf {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
      }

    }

  }

}