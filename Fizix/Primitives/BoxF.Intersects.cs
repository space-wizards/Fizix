using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsIntersectingNaive(BoxF a, BoxF b)
      => b.Left <= a.Right
        && b.Right >= a.Left
        && b.Top <= a.Bottom
        && b.Bottom >= a.Top;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsIntersectingSse(Vector128<float> a, Vector128<float> b) {
      var aMin = Sse.MoveLowToHigh(a, a);
      var aMax = Sse.MoveHighToLow(a, a);
      var bMin = Sse.MoveLowToHigh(b, b);
      var bMax = Sse.MoveHighToLow(b, b);
      var lt = Sse.CompareGreaterThan(aMin, bMax);
      var gt = Sse.CompareLessThan(aMax, bMin);
      var oob = Sse.Or(gt, lt);
      return Sse.MoveMask(oob) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIntersecting(Vector128<float> a, Vector128<float> b)
      => Sse.IsSupported
        ? IsIntersectingSse(a, b)
        : IsIntersectingNaive(a, b);

  }

}