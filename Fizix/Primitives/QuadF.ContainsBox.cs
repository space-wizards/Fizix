using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsBoxNaive(in QuadF q, in BoxF b) {
      var diff = q.Center - b.Center;
      MathF.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      PointF reoriented = (
        MathF.FusedMultiplyAdd(diff.X, cosTheta, diff.Y * -sinTheta),
        MathF.FusedMultiplyAdd(diff.X, sinTheta, diff.Y * cosTheta)
      );

      var qSize = q.Size;

      var relBox = new BoxF(-qSize, qSize);

      var bSize = b.Size;

      BoxF relOtherBox = (reoriented - bSize, reoriented + bSize);

      return relBox.Contains(relOtherBox);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsBox(in QuadF q, in BoxF b) => ContainsBoxNaive(q, b);

  }

}