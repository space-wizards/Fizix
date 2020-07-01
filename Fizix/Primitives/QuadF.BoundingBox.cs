using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BoundingBoxNaive(in QuadF q, out BoxF box) {
      q.GetCorners(out var a, out var b, out var c, out var d);

      box = new BoxF(
        MathF.Min(MathF.Min(a.X, b.X), MathF.Min(c.X, d.X)),
        MathF.Min(MathF.Min(a.Y, b.Y), MathF.Min(c.Y, d.Y)),
        MathF.Max(MathF.Max(a.X, b.X), MathF.Max(c.X, d.X)),
        MathF.Max(MathF.Max(a.Y, b.Y), MathF.Max(c.X, d.Y))
      );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BoundingBox(in QuadF q, out BoxF b)
      => BoundingBoxNaive(q, out b);

  }

}