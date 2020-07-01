using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IntersectsBoxNaive(in QuadF q, in BoxF b) {
      var diff = q.Center - b.Center;
      Math.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      var cosThetaF = (float)cosTheta;
      var sinThetaF = (float)sinTheta;
      
      var reoriented = new Vector2(
        MathF.FusedMultiplyAdd(diff.X, cosThetaF, diff.Y * -sinThetaF),
        MathF.FusedMultiplyAdd(diff.X, sinThetaF, diff.Y * cosThetaF)
      );

      var qSize = q.Size;

      var relBox = new BoxF(-qSize, qSize);

      var bSize = b.Size;

      var relOtherBox = new BoxF(reoriented - bSize, reoriented + bSize);

      return relBox.Intersects(relOtherBox);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IntersectsBox(in QuadF q, in BoxF b) => IntersectsBoxNaive(q, b);

  }

}