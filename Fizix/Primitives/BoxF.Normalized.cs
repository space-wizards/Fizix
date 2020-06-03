using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using CannyFastMath;
using MathF = CannyFastMath.MathF;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> NormalizedNaive(in Vector128<float> vr) {
      ref var r = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vr));
      float
        x1 = r.X1,
        y1 = r.Y1,
        x2 = r.X2,
        y2 = r.Y2;

      return (BoxF) (
        MathF.Min(x1, x2), MathF.Min(y1, y2),
        MathF.Max(x1, x2), MathF.Max(y1, y2)
      );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> NormalizedSse(in Vector128<float> r) {
      var xy1 = Sse.MoveLowToHigh(r, r);
      var xy2 = Sse.MoveHighToLow(r, r);
      var min = Sse.Min(xy1, xy2);
      var max = Sse.Max(xy1, xy2);
      return Sse.MoveLowToHigh(min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Normalized(in Vector128<float> r)
      => Sse.IsSupported
        ? NormalizedSse(r)
        : NormalizedNaive(r);

  }

}