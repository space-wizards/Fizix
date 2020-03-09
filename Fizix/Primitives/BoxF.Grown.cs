using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector128<float> GrownNaive(in Vector128<float> vr, float size) {
      ref var r = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vr));
      var half = size / 2;
      var result = (
        r.X1 - half,
        r.Y1 - half,
        r.X2 + half,
        r.Y2 + half
      );
      return Unsafe.As<ValueTuple<float, float, float, float>, Vector128<float>>(ref result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector128<float> GrownSse3(in Vector128<float> r, float size) {
      var half = Vector128.Create(size / -2);
      var ordered = Sse.Shuffle(r, r, 0b00_10_01_11);
      ordered = Sse3.AddSubtract(ordered, half);
      return Sse.Shuffle(ordered, ordered, 0b00_10_01_11);
    }
    
    public static Vector128<float> Grown(in Vector128<float> r, float p)
      => Sse3.IsSupported
        ? GrownSse3(r, p)
        : GrownNaive(r, p);

  }

}