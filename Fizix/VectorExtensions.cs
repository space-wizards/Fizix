using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public static class VectorExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector2 AsVector2(in this Vector64<float> a)
      => ref Unsafe.As<Vector64<float>, Vector2>(ref Unsafe.AsRef(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector4 AsVector4(in this Vector128<float> a)
      => ref Unsafe.As<Vector128<float>, Vector4>(ref Unsafe.AsRef(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector64<float> AsVector64Float(in this Vector2 a)
      => ref Unsafe.As<Vector2, Vector64<float>>(ref Unsafe.AsRef(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector128<float> AsVector128Float(in this Vector4 a)
      => ref Unsafe.As<Vector4, Vector128<float>>(ref Unsafe.AsRef(a));

  }

}