using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct PointF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF AddNaive(PointF a, PointF b)
      => new PointF(a.X + b.X, a.Y + b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF AddSse(PointF a, PointF b)
      => Sse.Add(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Add(PointF a, PointF b)
      => (Vector2) (Sse.IsSupported ? AddSse(a, b) : AddNaive(a, b));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF AddNaive(PointF a, SizeF b)
      => new PointF(a.X + b.Width, a.Y + b.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF AddSse(PointF a, SizeF b)
      => Sse.Add(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Add(PointF a, SizeF b)
      => (Vector2) (Sse.IsSupported ? AddSse(a, b) : AddNaive(a, b));

  }

}