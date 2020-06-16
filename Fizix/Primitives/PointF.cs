using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly partial struct PointF : IEquatable<PointF> {

#pragma warning disable 169, 649
    private readonly float _x, _y;
#pragma warning restore 169, 649

    public float X {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _x;
    }

    public float Y {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator -(PointF p)
      => Negate(p);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PointF(in Vector128<float> v)
      => new PointF(
        v.ToScalar(),
        v.GetElement(1)
      );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector128<float>(in PointF v) {
      var x = Vector128.CreateScalarUnsafe(v.X);
      if (!Sse.IsSupported)
        return x.WithElement(1, v.Y);

      var y = Vector128.CreateScalarUnsafe(v.Y);
      return Sse.UnpackLow(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<float, float>(in PointF v)
      => Unsafe.As<PointF, ValueTuple<float, float>>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PointF(in ValueTuple<float, float> v)
      => Unsafe.As<ValueTuple<float, float>, PointF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PointF(in Vector2 v)
      => Unsafe.As<Vector2, PointF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Vector2(in PointF v)
      => Unsafe.As<PointF, Vector2>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator PointF(in SizeF v)
      => Unsafe.As<SizeF, PointF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator +(PointF p, PointF v)
      => Add(p, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator +(PointF p, SizeF v)
      => Add(p, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator *(PointF p, float v)
      => Multiply(p, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PointF(float x, float y) {
      _x = x;
      _y = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator -(PointF p, PointF v)
      => Subtract(p, v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF operator -(PointF p, SizeF v)
      => Subtract(p, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PointF a, PointF b)
      => a.Equals(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PointF a, PointF b)
      => !a.Equals(b);

    // ReSharper disable CompareOfFloatsByEqualityOperator
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(PointF other)
      => X == other.X && Y == other.Y;
    // ReSharper restore CompareOfFloatsByEqualityOperator

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IEquatable<PointF>.Equals(PointF other)
      => Equals(other);

    public override bool Equals(object obj)
      => obj is PointF other && Equals(other);

    public override int GetHashCode()
      => HashCode.Combine(X, Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out float x, out float y) {
      x = X;
      y = Y;
    }

  }

}