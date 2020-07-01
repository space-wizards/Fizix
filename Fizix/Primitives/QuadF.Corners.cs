using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void CornersNaive(in QuadF q, out Vector2 tl, out Vector2 br, out Vector2 tr, out Vector2 bl) {
      MathF.SinCos(q.Angle, out var sinTheta, out var cosTheta);

      var qCenter = q.Center;
      var qSize = q.Size;

      var halfSize = qSize * .5f;
      var (qTlX, qTlY) = qCenter - halfSize;
      var (qBrX, qBrY) = qCenter + halfSize;
      var (qTrX, qTrY) = new Vector2(qCenter.X + halfSize.X, qCenter.Y - halfSize.Y);
      var (qBlX, qBlY) = new Vector2(qCenter.X - halfSize.X, qCenter.Y + halfSize.Y);

      tl = new Vector2(
        MathF.FusedMultiplyAdd(qTlX, cosTheta, qTlY * -sinTheta),
        MathF.FusedMultiplyAdd(qTlX, sinTheta, qTlY * cosTheta)
      );

      br = new Vector2(
        MathF.FusedMultiplyAdd(qBrX, cosTheta, qBrY * -sinTheta),
        MathF.FusedMultiplyAdd(qBrX, sinTheta, qBrY * cosTheta)
      );

      tr = new Vector2(
        MathF.FusedMultiplyAdd(qTrX, cosTheta, qTrY * -sinTheta),
        MathF.FusedMultiplyAdd(qTrX, sinTheta, qTrY * cosTheta)
      );

      bl = new Vector2(
        MathF.FusedMultiplyAdd(qBlX, cosTheta, qBlY * -sinTheta),
        MathF.FusedMultiplyAdd(qBlX, sinTheta, qBlY * cosTheta)
      );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Corners(in QuadF q, out Vector2 tl, out Vector2 br, out Vector2 tr, out Vector2 bl)
      => CornersNaive(q, out tl, out br, out tr, out bl);

  }

}