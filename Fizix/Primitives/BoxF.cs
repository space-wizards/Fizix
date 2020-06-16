using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly partial struct BoxF : IBoxF {

#pragma warning disable 169, 649
    private readonly Vector128<float> _value;

    public BoxF(float x1, float y1, float x2, float y2)
      => _value = Vector128.Create(x1, y1, x2, y2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoxF(PointF topLeft, PointF bottomRight) {
      if (Sse.IsSupported) {
        _value = Sse.MoveLowToHigh(topLeft, bottomRight);
      }

      _value = Vector128.Create(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoxF(PointF topLeft, SizeF widthHeight) {
      if (Sse.IsSupported) {
        _value = Sse.MoveLowToHigh(topLeft, Sse.Add(topLeft, widthHeight));
      }

      var (x, y) = topLeft + widthHeight;
      _value = Vector128.Create(topLeft.X, topLeft.Y, x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoxF(SizeF topLeft, SizeF bottomRight) {
      if (Sse.IsSupported) {
        _value = Sse.MoveLowToHigh(topLeft, bottomRight);
      }

      _value = Vector128.Create(topLeft.Width, topLeft.Height, bottomRight.Width, bottomRight.Height);
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

    public PointF TopLeft {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new PointF(X1, Y1);
    }

    public PointF BottomRight {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new PointF(X2, Y2);
    }

    public PointF Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        var tl = TopLeft;
        var bl = BottomRight;
        return (tl + bl) * .5f;
      }
    }

    public SizeF Size {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => GetSize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<float, float, float, float>(BoxF v)
      => Unsafe.As<BoxF, ValueTuple<float, float, float, float>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BoxF(ValueTuple<float, float, float, float> v)
      => Unsafe.As<ValueTuple<float, float, float, float>, BoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ValueTuple<Vector2, Vector2>(BoxF v)
      => Unsafe.As<BoxF, ValueTuple<Vector2, Vector2>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator BoxF(ValueTuple<Vector2, Vector2> v)
      => Unsafe.As<ValueTuple<Vector2, Vector2>, BoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<PointF, PointF>(BoxF v)
      => Unsafe.As<BoxF, ValueTuple<PointF, PointF>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BoxF(ValueTuple<PointF, PointF> v)
      => Unsafe.As<ValueTuple<PointF, PointF>, BoxF>(ref v);

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
      return (s.Width + s.Height) * 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Area(in BoxF x) {
      var s = x.Size;
      return s.Width * s.Height;
    }

    public override string ToString()
      => $"(T{Top}, L{Left}, B{Bottom}, R{Right})";

  }

}