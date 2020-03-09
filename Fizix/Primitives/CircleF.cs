using System.Numerics;
using System.Runtime.CompilerServices;
using CannyFastMath;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
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

    public bool Contains(PointF p)
      => (Center - p).LengthSquared() < RadiusSquared;

    public bool Intersects(CircleF circle)
    {
      var d = Center - circle.Center;
      var r = Radius + circle.Radius;
      return Vector2.Dot(d,d) < r * r;
    }
    
    public bool Intersects(BoxF box)
    {
      var closestX = MathF.Median(box.Left, box.Right, Center.X);
      var closestY = MathF.Median(box.Bottom, box.Top, Center.Y);
      return Contains((closestX, closestY));
    }

    public BoxF ContainingBox() {
      var r = new Vector2(Radius);
      return (BoxF) (Center - r, Center + r);
    }
  }

}