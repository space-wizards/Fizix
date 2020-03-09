using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsRectNaive(in Vector128<float> va, in Vector128<float> vb) {
      ref var a = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(va));
      ref var b = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vb));
      return
        b.Left >= a.Left
        && b.Right <= a.Right
        && b.Top >= a.Top
        && b.Bottom <= a.Bottom;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsRectSse(in Vector128<float> a, in Vector128<float> b) {
      var aMin = Sse.MoveLowToHigh(a, a);
      var aMax = Sse.MoveHighToLow(a, a);
      var bMin = Sse.MoveLowToHigh(b, b);
      var bMax = Sse.MoveHighToLow(b, b);
      var gt = Sse.CompareGreaterThan(aMin, bMin);
      var lt = Sse.CompareLessThan(aMax, bMax);
      var oob = Sse.Or(gt, lt);
      return Sse.MoveMask(oob) == 0;
    }

    public static bool ContainsRect(in Vector128<float> a, in Vector128<float> b)
      => Sse.IsSupported
        ? ContainsRectSse(a, b)
        : ContainsRectNaive(a, b);

  }

}