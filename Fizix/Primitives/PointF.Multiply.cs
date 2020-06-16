using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct PointF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF MultiplyNaive(PointF a, PointF b)
      => new PointF(a.X * b.X, a.Y * b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF MultiplySse(PointF a, PointF b)
      => Sse.Multiply(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Multiply(PointF a, PointF b)
      => (Vector2) (Sse.IsSupported ? MultiplySse(a, b) : MultiplyNaive(a, b));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF MultiplyNaive(PointF a, float b)
      => new PointF(a.X * b, a.Y * b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF MultiplySse(PointF a, float b)
      => Sse.Multiply(a, Vector128.Create(b));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Multiply(PointF a, float b)
      => (Vector2) (Sse.IsSupported ? MultiplySse(a, b) : MultiplyNaive(a, b));

  }

}