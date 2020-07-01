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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CollisionRayF(in RayF ray, T collisionMask) {
      Ray = ray;
      CollisionMask = collisionMask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CollisionRayF(Vector2 start, Vector2 heading, T collisionMask)
      : this(new RayF(start, heading), collisionMask) {
    }

    public Vector2 Start {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Ray.Start;
    }

    public Vector2 Heading {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Ray.Heading;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Intersects(in CollisionBoxF<T> box, out float distance, out Vector2 location)
      => Ray.Intersects(box, out distance, out location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator RayF(in CollisionRayF<T> ray)
      => Unsafe.As<CollisionRayF<T>, RayF>(ref Unsafe.AsRef(ray));

  }

}