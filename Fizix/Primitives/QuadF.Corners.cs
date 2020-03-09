using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void CornersNaive(in QuadF q, out PointF tl, out PointF br, out PointF tr, out PointF bl) {
      MathF.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      var qCenter = q.Center;
      var qSize = q.Size;

      var halfSize = qSize * .5f;
      var (qTlX, qTlY) = qCenter - halfSize;
      var (qBrX, qBrY) = qCenter + halfSize;
      var (qTrX, qTrY) = (PointF) (qCenter.X + halfSize.Width, qCenter.Y - halfSize.Height);
      var (qBlX, qBlY) = (PointF) (qCenter.X - halfSize.Width, qCenter.Y + halfSize.Height);

      tl = (
        MathF.FusedMultiplyAdd(qTlX, cosTheta, qTlY * -sinTheta),
        MathF.FusedMultiplyAdd(qTlX, sinTheta, qTlY * cosTheta)
      );

      br = (
        MathF.FusedMultiplyAdd(qBrX, cosTheta, qBrY * -sinTheta),
        MathF.FusedMultiplyAdd(qBrX, sinTheta, qBrY * cosTheta)
      );

      tr = (
        MathF.FusedMultiplyAdd(qTrX, cosTheta, qTrY * -sinTheta),
        MathF.FusedMultiplyAdd(qTrX, sinTheta, qTrY * cosTheta)
      );

      bl = (
        MathF.FusedMultiplyAdd(qBlX, cosTheta, qBlY * -sinTheta),
        MathF.FusedMultiplyAdd(qBlX, sinTheta, qBlY * cosTheta)
      );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Corners(in QuadF q, out PointF tl, out PointF br, out PointF tr, out PointF bl)
      => CornersNaive(q, out tl, out br, out tr, out bl);

  }

}