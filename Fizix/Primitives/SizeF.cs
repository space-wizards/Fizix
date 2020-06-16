using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly partial struct SizeF : IEquatable<SizeF> {

#pragma warning disable 169, 649
    private readonly float _width, _height;
#pragma warning restore 169, 649

    public float Width {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _width;
    }

    public float Height {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _height;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator -(SizeF p)
      => Negate(p);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SizeF(in Vector128<float> v)
      => new SizeF(
        v.ToScalar(),
        v.GetElement(1)
      );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector128<float>(in SizeF v) {
      var x = Vector128.CreateScalarUnsafe(v.Width);
      if (!Sse.IsSupported)
        return x.WithElement(1, v.Height);

      var y = Vector128.CreateScalarUnsafe(v.Height);
      return Sse.UnpackLow(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<float, float>(in SizeF v)
      => Unsafe.As<SizeF, ValueTuple<float, float>>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SizeF(in ValueTuple<float, float> v)
      => Unsafe.As<ValueTuple<float, float>, SizeF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SizeF(in Vector2 v)
      => Unsafe.As<Vector2, SizeF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Vector2(in SizeF v)
      => Unsafe.As<SizeF, Vector2>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator SizeF(in PointF v)
      => Unsafe.As<PointF, SizeF>(ref Unsafe.AsRef(v));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator +(SizeF p, SizeF v)
      => Add(p, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator *(SizeF p, float v)
      => Multiply(p, v);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SizeF(float width, float height) {
      _width = width;
      _height = height;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF operator -(SizeF p, SizeF v)
      => Subtract(p, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(SizeF a, SizeF b)
      => a.Equals(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(SizeF a, SizeF b)
      => !a.Equals(b);

    // ReSharper disable CompareOfFloatsByEqualityOperator
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(SizeF other)
      => Width == other.Width && Height == other.Height;

    public override bool Equals(object obj)
      => obj is SizeF other && Equals(other);

    public override int GetHashCode()
      => HashCode.Combine(Width, Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out float x, out float y) {
      x = Width;
      y = Height;
    }

  }

}