using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct PointF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF SubtractNaive(PointF a, PointF b)
      => new PointF(a.X - b.X, a.Y - b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF SubtractSse(PointF a, PointF b)
      => Sse.Subtract(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Subtract(PointF a, PointF b)
      => (Vector2) (Sse.IsSupported ? SubtractSse(a, b) : SubtractNaive(a, b));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF SubtractNaive(PointF a, SizeF b)
      => new PointF(a.X - b.Width, a.Y - b.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF SubtractSse(PointF a, SizeF b)
      => Sse.Subtract(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Subtract(PointF a, SizeF b)
      => (Vector2) (Sse.IsSupported ? SubtractSse(a, b) : SubtractNaive(a, b));

  }

}