using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsPointNaive(in Vector128<float> vr, in Vector64<float> vp) {
      ref var r = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vr));
      ref var p = ref Unsafe.As<Vector64<float>, PointF>(ref Unsafe.AsRef(vp));
      return
        r.Left <= p.X
        && r.Top <= p.Y
        && r.Right >= p.X
        && r.Bottom >= p.Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsPointSse(in Vector128<float> r, in Vector64<float> p) {
      var wp = p.WidenToVector128();
      var min = Sse.MoveLowToHigh(r, r);
      var max = Sse.MoveHighToLow(r, r);
      wp = Sse.MoveLowToHigh(wp, wp);
      var gt = Sse.CompareGreaterThan(wp, max);
      var lt = Sse.CompareLessThan(wp, min);
      var oob = Sse.Or(gt, lt);
      return Sse.MoveMask(oob) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsPoint(in Vector128<float> r, in Vector64<float> p)
      => Sse.IsSupported
        ? ContainsPointSse(r, p)
        : ContainsPointNaive(r, p);

  }

}