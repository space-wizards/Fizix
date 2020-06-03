using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> ScaledNaive(in Vector128<float> vr, float scale) {
      ref var r = ref Unsafe.As<Vector128<float>, Vector4>(ref Unsafe.AsRef(vr));
      var w = r.Z - r.X;
      var h = r.W - r.Y;
      var hv = new Vector2(w, h) * (scale * .5f);
      var tl = new Vector2(r.X, r.Y);
      var br = new Vector2(r.Z, r.W);
      var c = (tl + br) * .5f;
      var result = (c - hv, c + hv);
      return Unsafe.As<ValueTuple<Vector2, Vector2>, Vector128<float>>(ref result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> ScaledSse3(in Vector128<float> r, float scale) {
      var half = Vector128.Create(.5f);
      var halfScale = Vector128.Create(scale);
      halfScale = Sse.Multiply(halfScale, half);
      var ordered = Sse.Shuffle(r, r, 0b01110010);

      var center = Sse3.HorizontalAdd(ordered, ordered);
      center = Sse.Multiply(center, half);

      var size = Sse3.HorizontalSubtract(ordered, ordered);
      var scaledSize = Sse.Multiply(size, halfScale);

      center = Sse.Shuffle(center, center, 0b11_01_10_00);
      scaledSize = Sse.Shuffle(scaledSize, scaledSize, 0b11_01_10_00);
      var scaled = Sse3.AddSubtract(center, scaledSize);

      return Sse.Shuffle(scaled, scaled, 0b11_01_10_00);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Scaled(in Vector128<float> r, float p)
      => Sse3.IsSupported
        ? ScaledSse3(r, p)
        : ScaledNaive(r, p);

  }

}