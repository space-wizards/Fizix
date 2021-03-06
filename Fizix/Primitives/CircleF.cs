using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MathF = CannyFastMath.MathF;

namespace Fizix {

  [PublicAPI]
  [Serializable]
  public readonly struct CircleF {

    public Vector2 Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public float Radius {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public float RadiusSquared {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Radius * Radius;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Vector2 p)
      => (Center - p).LengthSquared() < RadiusSquared;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Intersects(CircleF circle) {
      var d = Center - circle.Center;
      var r = Radius + circle.Radius;
      return Vector2.Dot(d, d) < r * r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Intersects(BoxF box) {
      var closestX = MathF.Median(box.Left, box.Right, Center.X);
      var closestY = MathF.Median(box.Bottom, box.Top, Center.Y);
      return Contains(new Vector2(closestX, closestY));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoxF ContainingBox() {
      var r = new Vector2(Radius);
      return new BoxF(Center - r, Center + r);
    }

  }

}