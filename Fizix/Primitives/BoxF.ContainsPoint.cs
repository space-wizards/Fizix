using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsPointNaive(in BoxF r, Vector2 p)
      => r.Left <= p.X
        && r.Top <= p.Y
        && r.Right >= p.X
        && r.Bottom >= p.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsPointSse(in BoxF r, Vector128<float> p) {
      var min = Sse.MoveLowToHigh(r, r);
      var max = Sse.MoveHighToLow(r, r);
      var pt = Sse.MoveLowToHigh(p, p);
      var gt = Sse.CompareGreaterThan(pt, max);
      var lt = Sse.CompareLessThan(pt, min);
      var oob = Sse.Or(gt, lt);
      return Sse.MoveMask(oob) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsPoint(in BoxF r, Vector2 p)
      => Sse.IsSupported
        ? ContainsPointSse(r, p.ToVector128())
        : ContainsPointNaive(r, p);

  }

}