using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly partial struct BoxF {

#pragma warning disable 169, 649
    private readonly Vector128<float> _value;

    public BoxF(float x1, float y1, float x2, float y2)
      => _value = Vector128.Create(x1, y1, x2, y2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoxF(Vector2 topLeft, Vector2 bottomRight) {
      if (Sse.IsSupported) {
        var tl = topLeft.ToVector128();
        var br = bottomRight.ToVector128();
        _value = Sse.MoveLowToHigh(tl, br);
        return;
      }

      _value = Vector128.Create(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
    }

#pragma warning restore 169, 649

    public float X1 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.GetElement(0);
    }

    public float Y1 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.GetElement(1);
    }

    public float X2 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.GetElement(2);
    }

    public float Y2 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.GetElement(3);
    }

    public float Top {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Y1;
    }

    public float Left {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => X1;
    }

    public float Bottom {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Y2;
    }

    public float Right {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => X2;
    }

    public float Width {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => X2 - X1;
    }

    public float Height {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Y2 - Y1;
    }

    public Vector2 TopLeft {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new Vector2(X1, Y1);
    }

    public Vector2 BottomRight {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new Vector2(X2, Y2);
    }

    public Vector2 Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        var tl = TopLeft;
        var bl = BottomRight;
        return (tl + bl) * .5f;
      }
    }

    public Vector2 Size {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => GetSize(this);
    }

    /*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UiBoxF(BoxF v)
      => Unsafe.As<BoxF, UiBoxF>(ref v);
    */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BoxF(UiBoxF v)
      => Unsafe.As<UiBoxF, BoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector128<float>(BoxF v)
      => Unsafe.As<BoxF, Vector128<float>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BoxF(Vector128<float> v)
      => Unsafe.As<Vector128<float>, BoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector4(BoxF v)
      => Unsafe.As<BoxF, Vector4>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BoxF(Vector4 v)
      => Unsafe.As<Vector4, BoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Perimeter(in BoxF x) {
      var s = x.Size;
      return (s.X + s.Y) * 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Area(in BoxF x) {
      var s = x.Size;
      return s.X * s.Y;
    }

    public override string ToString()
      => $"(T{Top}, L{Left}, B{Bottom}, R{Right})";

  }

}