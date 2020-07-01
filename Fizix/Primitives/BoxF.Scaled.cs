using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BoxF ScaledNaive(Vector4 r, float scale) {
      var w = r.Z - r.X;
      var h = r.W - r.Y;
      var hv = new Vector2(w, h) * (scale * .5f);
      var tl = new Vector2(r.X, r.Y);
      var br = new Vector2(r.Z, r.W);
      var c = (tl + br) * .5f;
      return new BoxF(c - hv, c + hv);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BoxF ScaledSse3(Vector128<float> r, float scale) {
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
    public static BoxF Scaled(BoxF r, float p)
      => Sse3.IsSupported
        ? ScaledSse3(r, p)
        : ScaledNaive(r, p);

  }

}