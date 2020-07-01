using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector2 GetSizeNaive(in BoxF r)
      => new Vector2(r.Width, r.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector2 GetSizeSse(Vector128<float> r) {
      var l = r;
      var h = Sse.MoveHighToLow(l, l);
      return Sse.Subtract(h, l).ToVector2();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetSize(BoxF r)
      => Sse.IsSupported
        ? GetSizeSse(r)
        : GetSizeNaive(r);

  }

}