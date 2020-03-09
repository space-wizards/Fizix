using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly struct CollisionRayF<T> where T : struct {

    public RayF Ray {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public T CollisionMask {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public CollisionRayF(in RayF ray, in T collisionMask)
      => (Ray, CollisionMask) = (ray, collisionMask);

    public Vector2 Start {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Ray.Start;
    }

    public Vector2 Heading {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Ray.Heading;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Intersects(in CollisionBoxF<T> box, out float distance, out PointF location)
      => Ray.Intersects(box, out distance, out location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator RayF(in CollisionRayF<T> ray)
      => Unsafe.As<CollisionRayF<T>, RayF>(ref Unsafe.AsRef(ray));

  }

}