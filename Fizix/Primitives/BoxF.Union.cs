using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector128<float> UnionedNaive(in Vector128<float> va, in Vector128<float> vb) {
      ref var aTopLeft = ref Unsafe.As<Vector128<float>, Vector2>(ref Unsafe.AsRef(va));
      ref var aBottomRight = ref Unsafe.Add(ref aTopLeft, 1);
      ref var bTopLeft = ref Unsafe.As<Vector128<float>, Vector2>(ref Unsafe.AsRef(vb));
      ref var bBottomRight = ref Unsafe.Add(ref bTopLeft, 1);
      var result = (
        Vector2.Min(aTopLeft, bTopLeft),
        Vector2.Max(aBottomRight, bBottomRight)
      );
      return Unsafe.As<ValueTuple<Vector2, Vector2>, Vector128<float>>(ref result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector128<float> UnionedSse(in Vector128<float> a, in Vector128<float> b) {
      var min = Sse.Min(a, b);
      var max = Sse.Max(a, b);
      return Sse.Shuffle(min, max, 0b11_10_01_00);
    }

    public static Vector128<float> Unioned(in Vector128<float> a, in Vector128<float> b)
      => Sse.IsSupported
        ? UnionedSse(a, b)
        : UnionedNaive(a, b);

  }

}