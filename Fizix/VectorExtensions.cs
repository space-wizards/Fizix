using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public static class VectorExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector4 AsVector4(in this Vector128<float> a)
      => ref Unsafe.As<Vector128<float>, Vector4>(ref Unsafe.AsRef(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector128<float> AsVector128Float(in this Vector4 a)
      => ref Unsafe.As<Vector4, Vector128<float>>(ref Unsafe.AsRef(a));

    public static Vector128<float> ToVector128(this Vector2 v) {
      var x = Vector128.CreateScalarUnsafe(v.X);
      if (!Sse.IsSupported)
        return x.WithElement(1, v.Y);

      var y = Vector128.CreateScalarUnsafe(v.Y);
      return Sse.UnpackLow(x, y);
    }

    public static Vector2 ToVector2(this Vector128<float> v) {
      var x = v.GetElement(0);
      var y = v.GetElement(1);
      return new Vector2(x, y);
    }

    public static void Deconstruct(this Vector2 v, out float x, out float y) {
      x = v.X;
      y = v.Y;
    }

  }

}