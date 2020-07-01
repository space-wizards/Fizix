using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly partial struct QuadF {

    public Vector2 Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public Vector2 Size {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public double Angle {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    private QuadF(Vector2 center, Vector2 size, double angle)
      => (Center, Size, Angle) = (center, size, angle);

    public float Width {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Size.X;
    }

    public float Height {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Size.Y;
    }

    public BoxF Box {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new BoxF(Center - Size, Center + Size);
    }

  }

}