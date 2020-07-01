using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasNaNNaive(in BoxF r)
      => float.IsNaN(MathF.FusedMultiplyAdd(r.X1, r.Y1, r.X2 * r.Y2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasNaNSse(in BoxF r)
      => Sse.MoveMask(Sse.CompareUnordered(r, default)) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsNaN(in BoxF r)
      => Sse.IsSupported
        ? HasNaNSse(r)
        : HasNaNNaive(r);

  }

}