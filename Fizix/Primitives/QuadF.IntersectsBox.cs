using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IntersectsBoxNaive(in QuadF q, in BoxF b) {
      var (diffX, diffY) = q.Center - b.Center;
      MathF.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      PointF reoriented = (
        MathF.FusedMultiplyAdd(diffX, cosTheta, diffY * -sinTheta),
        MathF.FusedMultiplyAdd(diffX, sinTheta, diffY * cosTheta)
      );

      Vector2 qSize = q.Size;

      BoxF relBox = (-qSize, qSize);

      var bSize = b.Size;

      BoxF relOtherBox = (reoriented - bSize, reoriented + bSize);

      return relBox.Intersects(relOtherBox);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IntersectsBox(in QuadF q, in BoxF b) => IntersectsBoxNaive(q, b);

  }

}