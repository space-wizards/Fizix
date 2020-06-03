using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsIntersectingNaive(in Vector128<float> va, in Vector128<float> vb) {
      ref var a = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(va));
      ref var b = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vb));
      return
        b.Left <= a.Right
        && b.Right >= a.Left
        && b.Top <= a.Bottom
        && b.Bottom >= a.Top;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsIntersectingSse(in Vector128<float> a, in Vector128<float> b) {
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
    public static bool IsIntersecting(in Vector128<float> a, in Vector128<float> b)
      => Sse.IsSupported
        ? IsIntersectingSse(a, b)
        : IsIntersectingNaive(a, b);

  }

}