using System;
using System.Numerics;
using JetBrains.Annotations;
using MathF = CannyFastMath.MathF;

namespace Fizix {

  [PublicAPI]
  [Serializable]
  public static class LineSegF {

    public static bool Intersection(Vector2 aStart, Vector2 aEnd, Vector2 bStart, Vector2 bEnd, out Vector2 intersection) {
      Vector2 aSize = aEnd - aStart, bSize = bEnd - bStart;

      var cross = aSize.X * bSize.Y - aSize.Y * bSize.X;

      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (cross == 0f) {
        intersection = default;
        return false;
      }

      var crossReciprocal = 1.0f / cross;

      var distStart = bStart - aStart;

      var negDistStartY = -distStart.Y;

      var checkA = MathF.FusedMultiplyAdd(distStart.X, aSize.Y, negDistStartY * aSize.X) * crossReciprocal;
      var checkB = MathF.FusedMultiplyAdd(distStart.X, bSize.Y, negDistStartY * bSize.X) * crossReciprocal;

      if (checkA < 0 || checkB < 0 || checkA > 1 || checkB > 1) {
        intersection = default;
        return false;
      }

      intersection = new Vector2(
        MathF.FusedMultiplyAdd(checkB, aSize.X, aStart.X),
        MathF.FusedMultiplyAdd(checkB, aSize.Y, aStart.Y)
      );
      return true;
    }

  }

}