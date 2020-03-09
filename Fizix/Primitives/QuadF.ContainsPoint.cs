using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsPointNaive(in QuadF q, in PointF p) {
      var diff = q.Center - p;
      MathF.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      PointF reoriented = (
        MathF.FusedMultiplyAdd(diff.X, cosTheta, diff.Y * -sinTheta),
        MathF.FusedMultiplyAdd(diff.X, sinTheta, diff.Y * cosTheta)
      );

      Vector2 size = q.Size;

      BoxF relBox = (-size, size);

      return relBox.Contains(reoriented);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsPoint(in QuadF q, in PointF p) => ContainsPointNaive(q, p);

  }

}