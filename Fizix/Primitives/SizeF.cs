using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly struct SizeF : IEquatable<SizeF> {

#pragma warning disable 169, 649
    private readonly Vector2 _value;
#pragma warning restore 169, 649

    public float Width {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.X;
    }

    public float Height {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.Y;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<float, float>(in SizeF v)
      => Unsafe.As<SizeF, ValueTuple<float, float>>(ref Unsafe.AsRef( v ));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SizeF(in ValueTuple<float, float> v)
      => Unsafe.As<ValueTuple<float, float>, SizeF>(ref Unsafe.AsRef( v ));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector64<float>(in SizeF v)
      => Unsafe.As<SizeF, Vector64<float>>(ref Unsafe.AsRef( v ));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SizeF(in Vector64<float> v)
      => Unsafe.As<Vector64<float>, SizeF>(ref Unsafe.AsRef( v ));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(in SizeF v)
      => Unsafe.As<SizeF, Vector2>(ref Unsafe.AsRef( v ));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SizeF(in Vector2 v)
      => Unsafe.As<Vector2, SizeF>(ref Unsafe.AsRef( v ));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator SizeF(in PointF v)
      => Unsafe.As<PointF, SizeF>(ref Unsafe.AsRef( v ));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator PointF(in SizeF v)
      => Unsafe.As<SizeF, PointF>(ref Unsafe.AsRef( v ));

    public static SizeF operator +(in SizeF p, in Vector2 v)
      => Vector2.Add(p,v);

    public static SizeF operator -(in SizeF p, in Vector2 v)
      => Vector2.Subtract(p,v);

    public static SizeF operator +(in SizeF p, float v)
      => Vector2.Add(p,  new Vector2(v));

    public static SizeF operator -(in SizeF p, float v)
      => Vector2.Subtract(p, new Vector2(v));

    public static SizeF operator *(in SizeF p, float v)
      => Vector2.Multiply(p, new Vector2(v));

    public static SizeF operator /(in SizeF p, float v)
      => Vector2.Divide(p, new Vector2(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in SizeF a, in SizeF b)
      => a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in SizeF a, in SizeF b)
      => !a.Equals(b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in SizeF other)
      => _value.Equals(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IEquatable<SizeF>.Equals(SizeF other)
      => _value.Equals(other._value);
    
    public override bool Equals(object obj)
      => obj is SizeF other && Equals(other);

    public override int GetHashCode()
      => _value.GetHashCode();

    public void Deconstruct(out float w, out float h)
      => (w, h) = (Width, Height);

  }

}