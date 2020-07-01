using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MathF = CannyFastMath.MathF;

namespace Fizix {

  [PublicAPI]
  public readonly partial struct RayF {

    public Vector2 Start {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public Vector2 Heading {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RayF(Vector2 start, Vector2 heading) {
      Start = start;
      Heading = heading;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static bool IntersectsBoxNaive(in RayF ray, in BoxF box, out float distance, out Vector2 location, float epsilon = 1e-7f) {
      var tMin = 0.0f; // set to -FLT_MAX to get first hit on line
      var tMax = float.MaxValue; // set to max distance ray can travel (for segment)

      var start = ray.Start;
      var heading = ray.Heading;

      var directionX = heading.X;
      var positionX = start.X;
      var boxLeft = box.Left;
      var boxRight = box.Right;

      if (MathF.Abs(directionX) < epsilon) // ray is parallel to this slab, it will never hit unless ray is inside box
        if (positionX < boxLeft
          || positionX > boxRight) {
          distance = default;
          location = default;
          return false;
        }

      var directionY = heading.Y;
      var positionY = start.Y;
      var boxTop = box.Top;
      var boxBottom = box.Bottom;

      if (MathF.Abs(directionY) < epsilon) // ray is parallel to this slab, it will never hit unless ray is inside box
        if (positionY < boxTop
          || positionY > boxBottom) {
          distance = default;
          location = default;
          return false;
        }

      {
        // calculate intersection t value of ray with near and far plane of slab
        var rD = 1 / directionX;
        var t1 = rD * (boxLeft - positionX);
        var t2 = rD * (boxRight - positionX);

        if (t1 <= t2) {
          tMin = t1 > tMin ? t1 : tMin;
          tMax = t2 < tMax ? t2 : tMax;
        }
        else {
          tMin = t2 > tMin ? t2 : tMin;
          tMax = t1 < tMax ? t1 : tMax;
        }

        // Exit with no collision as soon as slab intersection becomes empty
        if (tMin > tMax) {
          distance = default;
          location = default;
          return false;
        }
      }

      {
        // calculate intersection t value of ray with near and far plane of slab
        var rD = 1 / directionY;
        var t1 = rD * (boxTop - positionY);
        var t2 = rD * (boxBottom - positionY);

        if (t1 <= t2) {
          tMin = t1 > tMin ? t1 : tMin;
          tMax = t2 < tMax ? t2 : tMax;
        }
        else {
          tMin = t2 > tMin ? t2 : tMin;
          tMax = t1 < tMax ? t1 : tMax;
        }

        // Exit with no collision as soon as slab intersection becomes empty
        if (tMin > tMax) {
          distance = default;
          location = default;
          return false;
        }
      }
      // Ray intersects all slabs. Return point and intersection t value
      distance = tMin;
      location =
        new Vector2(
          MathF.FusedMultiplyAdd(heading.X, tMin, heading.X),
          MathF.FusedMultiplyAdd(heading.Y, tMin, heading.Y)
        );
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IntersectsBox(in RayF ray, in BoxF box, out float distance, out Vector2 location, float epsilon = 1e-7f)
      => IntersectsBoxNaive(ray, box, out distance, out location, epsilon);

  }

}