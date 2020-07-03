using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void CornersNaive(in QuadF q, out Vector2 tl, out Vector2 br, out Vector2 tr, out Vector2 bl) {
      Math.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      var qCenter = q.Center;
      var qSize = q.Size;

      var halfSize = qSize * .5f;
      var (qTlX, qTlY) = qCenter - halfSize;
      var (qBrX, qBrY) = qCenter + halfSize;
      var (qTrX, qTrY) = new Vector2(qCenter.X + halfSize.X, qCenter.Y - halfSize.Y);
      var (qBlX, qBlY) = new Vector2(qCenter.X - halfSize.X, qCenter.Y + halfSize.Y);

      var cosThetaF = (float) cosTheta;
      var sinThetaF = (float) sinTheta;

      tl = new Vector2(
        MathF.FusedMultiplyAdd(qTlX, cosThetaF, qTlY * -sinThetaF),
        MathF.FusedMultiplyAdd(qTlX, sinThetaF, qTlY * cosThetaF)
      );

      br = new Vector2(
        MathF.FusedMultiplyAdd(qBrX, cosThetaF, qBrY * -sinThetaF),
        MathF.FusedMultiplyAdd(qBrX, sinThetaF, qBrY * cosThetaF)
      );

      tr = new Vector2(
        MathF.FusedMultiplyAdd(qTrX, cosThetaF, qTrY * -sinThetaF),
        MathF.FusedMultiplyAdd(qTrX, sinThetaF, qTrY * cosThetaF)
      );

      bl = new Vector2(
        MathF.FusedMultiplyAdd(qBlX, cosThetaF, qBlY * -sinThetaF),
        MathF.FusedMultiplyAdd(qBlX, sinThetaF, qBlY * cosThetaF)
      );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Corners(in QuadF q, out Vector2 tl, out Vector2 br, out Vector2 tr, out Vector2 bl)
      => CornersNaive(q, out tl, out br, out tr, out bl);

  }

}