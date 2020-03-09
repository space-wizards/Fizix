using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FiniteNaive(in Vector128<float> vr) {
      ref var r = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vr));
      return float.IsFinite(MathF.FusedMultiplyAdd(r.X2, r.Y2, r.X1 * r.Y1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FiniteSse(in Vector128<float> r) {
      var r1 = r;
      var r2 = Sse.Add(r1, Vector128.Create(1f));
      var r3 = Sse.Add(r2, r2);
      var x = Sse.CompareEqual(r1, r3);
      var m = Sse.MoveMask(x);
      return m == 0;
    }

    public static bool Finite(in Vector128<float> r)
      => Sse.IsSupported
        ? FiniteSse(r)
        : FiniteNaive(r);

  }

}