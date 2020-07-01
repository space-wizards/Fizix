using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public static class Vector2Extensions {

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
      return new Vector2(x,y);
    }

    public static void Deconstruct(this Vector2 v, out float x, out float y) {
      x = v.X;
      y = v.Y;
    }
    
  }

}