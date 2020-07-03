using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MathF = CannyFastMath.MathF;

namespace Fizix {

  [PublicAPI]
  [Serializable]
  public static class PolygonF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2[] Create(params Vector2[] path) => path;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static bool ContainsPoint(this Vector2[] poly, Vector2 point) {
      var result = false;
      var j = poly.Length - 1;
      var pY = point.Y;
      var pX = point.X;
      for (var i = 0; i < poly.Length; i++) {
        var iY = poly[i].Y;
        var jY = poly[j].Y;
        var iX = poly[i].X;
        var jX = poly[j].X;

        if (iY < pY && jY >= pY
          || jY < pY && iY >= pY
          && MathF.FusedMultiplyAdd((pY - iY) / (jY - iY), jX - iX, iX)
          < pX)
          result = !result;
        j = i;
      }

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this Vector2[] poly, params Vector2[] other) {
      for (var i = 0; i < other.Length; ++i) {
        ref var point = ref other[i];
        if (!poly.ContainsPoint(point))
          return false;
      }

      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static bool TriContains(Vector2 t, Vector2 br, Vector2 bl, Vector2 point) {
      var result = false;
      var (pX, pY) = point;
      var (aX, aY) = t;
      var (bX, bY) = br;
      var (cX, cY) = bl;

      if (aY < pY && cY >= pY
        || cY < pY && aY >= pY
        && MathF.FusedMultiplyAdd((pY - aY) / (cY - aY), cX - aX, aX)
        < pX)
        result = true;

      if (bY < pY && aY >= pY
        || aY < pY && bY >= pY
        && MathF.FusedMultiplyAdd((pY - bY) / (aY - bY), aX - bX, bX)
        < pX)
        result = !result;

      if (cY < pY && bY >= pY
        || bY < pY && cY >= pY
        && MathF.FusedMultiplyAdd((pY - cY) / (bY - cY), bX - cX, cX)
        < pX)
        result = !result;

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static bool QuadContains(Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl, Vector2 point) {
      var result = false;
      var (pX, pY) = point;
      var (aX, aY) = tl;
      var (bX, bY) = tr;
      var (cX, cY) = br;
      var (dX, dY) = bl;

      if (aY < pY && dY >= pY
        || dY < pY && aY >= pY
        && MathF.FusedMultiplyAdd((pY - aY) / (dY - aY), dX - aX, aX)
        < pX)
        result = true;

      if (bY < pY && aY >= pY
        || aY < pY && bY >= pY
        && MathF.FusedMultiplyAdd((pY - bY) / (aY - bY), aX - bX, bX)
        < pX)
        result = !result;

      if (cY < pY && bY >= pY
        || bY < pY && cY >= pY
        && MathF.FusedMultiplyAdd((pY - cY) / (bY - cY), bX - cX, cX)
        < pX)
        result = !result;

      if (dY < pY && cY >= pY
        || cY < pY && dY >= pY
        && MathF.FusedMultiplyAdd((pY - dY) / (cY - dY), cX - dX, dX)
        < pX)
        result = !result;

      return result;
    }

  }

}