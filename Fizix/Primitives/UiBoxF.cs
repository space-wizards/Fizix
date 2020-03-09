using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Fizix {

  public struct UiBoxF : IBoxF {

#pragma warning disable 649
    private BoxF _box;
#pragma warning restore 649

    public float X1
      => _box.X1;

    public float Y1
      => _box.Y1;

    public float X2
      => _box.X2;

    public float Y2
      => _box.Y2;

    public float Top
      => _box.Bottom;

    public float Left
      => _box.Left;

    public float Bottom
      => _box.Top;

    public float Right
      => _box.Right;

    public float Width
      => _box.Width;

    public float Height
      => _box.Height;

    public PointF BottomLeft
      => _box.TopLeft;

    public PointF TopRight
      => _box.BottomRight;

    public PointF Center
      => _box.Center;

    public SizeF Size
      => _box.Size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<float, float, float, float>(UiBoxF v)
      => Unsafe.As<UiBoxF, ValueTuple<float, float, float, float>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UiBoxF(ValueTuple<float, float, float, float> v)
      => Unsafe.As<ValueTuple<float, float, float, float>, UiBoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ValueTuple<Vector2,Vector2>(UiBoxF v)
      => Unsafe.As<UiBoxF, ValueTuple<Vector2,Vector2>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UiBoxF(ValueTuple<Vector2,Vector2> v)
      => Unsafe.As<ValueTuple<Vector2,Vector2>, UiBoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<PointF,PointF>(UiBoxF v)
      => Unsafe.As<UiBoxF, ValueTuple<PointF,PointF>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UiBoxF(ValueTuple<PointF,PointF> v)
      => Unsafe.As<ValueTuple<PointF,PointF>, UiBoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector128<float>(UiBoxF v)
      => Unsafe.As<UiBoxF, Vector128<float>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UiBoxF(Vector128<float> v)
      => Unsafe.As<Vector128<float>, UiBoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator BoxF(UiBoxF v)
      => Unsafe.As<UiBoxF, BoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UiBoxF(Vector4 v)
      => Unsafe.As<Vector4, UiBoxF>(ref v);

    public override string ToString()
      => $"(B{Bottom}, L{Left}, T{Top}, R{Right})";
  }

}