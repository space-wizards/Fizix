using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static BoxF UnionedNaive(BoxF a, BoxF b) {
      var aTopLeft = a.TopLeft;
      var aBottomRight = a.BottomRight;
      var bTopLeft = b.TopLeft;
      var bBottomRight = b.BottomRight;
      return new BoxF(
        Vector2.Min(aTopLeft, bTopLeft),
        Vector2.Max(aBottomRight, bBottomRight)
      );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static BoxF UnionedSse(Vector128<float> a, Vector128<float> b) {
      var min = Sse.Min(a, b);
      var max = Sse.Max(a, b);
      return Sse.Shuffle(min, max, 0b11_10_01_00);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoxF Unioned(BoxF a, BoxF b)
      => Sse.IsSupported
        ? UnionedSse(a, b)
        : UnionedNaive(a, b);

  }

}