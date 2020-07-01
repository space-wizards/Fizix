using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsPointNaive(in QuadF q, Vector2 p) {
      var diff = q.Center - p;
      MathF.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      var reoriented = new Vector2(
        MathF.FusedMultiplyAdd(diff.X, cosTheta, diff.Y * -sinTheta),
        MathF.FusedMultiplyAdd(diff.X, sinTheta, diff.Y * cosTheta)
      );

      var size = q.Size;

      var relBox = new BoxF(-size, size);

      return relBox.Contains(reoriented);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsPoint(in QuadF q, Vector2 p) => ContainsPointNaive(q, p);

  }

}