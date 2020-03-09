using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly struct PointF : IEquatable<PointF> {

#pragma warning disable 169, 649
    private readonly Vector2 _value;
#pragma warning restore 169, 649
    
    public float X {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.X;
    }

    public float Y {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.Y;
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator -(in PointF p)
      => Vector2.Negate(p);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<float, float>(in PointF v)
      => Unsafe.As<PointF, ValueTuple<float, float>>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PointF(in ValueTuple<float, float> v)
      => Unsafe.As<ValueTuple<float, float>, PointF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector64<float>(in PointF v)
      => Unsafe.As<PointF, Vector64<float>>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PointF(in Vector64<float> v)
      => Unsafe.As<Vector64<float>, PointF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(in PointF v)
      => Unsafe.As<PointF, Vector2>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PointF(in Vector2 v)
      => Unsafe.As<Vector2, PointF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator SizeF(in PointF v)
      => Unsafe.As<PointF, SizeF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator PointF(in SizeF v)
      => Unsafe.As<SizeF, PointF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator +(in PointF p, in Vector2 v)
      => Vector2.Add(p,v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator -(in PointF p, in Vector2 v)
      => Vector2.Subtract(p,v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator +(in PointF p, in SizeF v)
      => Vector2.Add(p,v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator -(in PointF p, in SizeF v)
      => Vector2.Subtract(p,v);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in PointF a, in PointF b)
      => a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in PointF a, in PointF b)
      => !a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in PointF other)
      => _value.Equals(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IEquatable<PointF>.Equals(PointF other)
      => _value.Equals(other._value);
    
    public override bool Equals(object obj)
      => obj is PointF other && Equals(other);

    public override int GetHashCode()
      => _value.GetHashCode();

    public void Deconstruct(out float x, out float y)
      => (x, y) = (X, Y);

  }

}