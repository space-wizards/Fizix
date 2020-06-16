using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly partial struct QuadF {

    public PointF Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public SizeF Size {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public float Angle {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    private QuadF(PointF center, SizeF size, float angle)
      => (Center, Size, Angle) = (center, size, angle);

    public float Width {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Size.Width;
    }

    public float Height {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Size.Height;
    }

    public BoxF Box {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new BoxF(Center - Size, Center + Size);
    }

  }

}