using System.Runtime.CompilerServices;
using CannyFastMath;

namespace Fizix {

  public readonly partial struct QuadF {

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static bool IntersectsQuadNaive(in QuadF q, in QuadF o) {
      if (Unsafe.AreSame(ref Unsafe.AsRef(q), ref Unsafe.AsRef(o)))
        return false;

      q.GetCorners(out var qTl, out var qBr, out var qTr, out var qBl);
      o.GetCorners(out var oTl, out var oBr, out var oTr, out var oBl);

      var bQ = new BoxF(
        MathF.Min(MathF.Min(qTl.X, qBr.X), MathF.Min(qTr.X, qBl.X)),
        MathF.Min(MathF.Min(qTl.Y, qBr.Y), MathF.Min(qTr.Y, qBl.Y)),
        MathF.Max(MathF.Max(qTl.X, qBr.X), MathF.Max(qTr.X, qBl.X)),
        MathF.Max(MathF.Max(qTl.Y, qBr.Y), MathF.Max(qTr.X, qBl.Y))
      );

      var bO = new BoxF(
        MathF.Min(MathF.Min(oTl.X, oBr.X), MathF.Min(oTr.X, oBl.X)),
        MathF.Min(MathF.Min(oTl.Y, oBr.Y), MathF.Min(oTr.Y, oBl.Y)),
        MathF.Max(MathF.Max(oTl.X, oBr.X), MathF.Max(oTr.X, oBl.X)),
        MathF.Max(MathF.Max(oTl.Y, oBr.Y), MathF.Max(oTr.X, oBl.Y))
      );

      if (!bQ.Intersects(bO))
        return false;

      return PolygonF.QuadContains(qTl, qTr, qBr, qBl, oTl)
        || PolygonF.QuadContains(qTl, qTr, qBr, qBl, oTr)
        || PolygonF.QuadContains(qTl, qTr, qBr, qBl, oBr)
        || PolygonF.QuadContains(qTl, qTr, qBr, qBl, oBl);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IntersectsQuad(in QuadF q, in QuadF o) => ContainsQuadNaive(q, o);

  }

}