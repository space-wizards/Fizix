using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasNaNNaive(in Vector128<float> vr) {
      ref var r = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vr));
      return float.IsNaN(MathF.FusedMultiplyAdd(r.X1, r.Y1, r.X2 * r.Y2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasNaNSse(in Vector128<float> r)
      => Sse.MoveMask(Sse.CompareUnordered(r, default)) != 0;

    public static bool ContainsNaN(in Vector128<float> r)
      => Sse.IsSupported
        ? HasNaNSse(r)
        : HasNaNNaive(r);

  }

}