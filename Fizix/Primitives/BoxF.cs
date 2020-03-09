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
    private readonly Vector4 _value;
#pragma warning restore 169, 649

    public float X1 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.X;
    }

    public float Y1 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.Y;
    }

    public float X2 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.Z;
    }

    public float Y2 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _value.W;
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
      get {
        var v = (Vector128<float>) this;
        return Unsafe.As<Vector128<float>,PointF>(ref v);
      }
    }

    public PointF BottomRight {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        var v = (Vector128<float>) this;
        return Unsafe.Add( ref Unsafe.As<Vector128<float>,PointF>(ref v), 1);
      }
    }

    public PointF Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        var v = (Vector128<float>) this;
        ref var tl = ref Unsafe.As<Vector128<float>,Vector2>(ref v);
        ref var bl = ref Unsafe.Add( ref tl, 1);
        return (tl + bl) / 2;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector64<float> GetSizeNaive(Vector128<float> vr) {
      ref var r = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vr));
      var v = (r.Width, r.Height);
      return Unsafe.As<ValueTuple<float, float>, Vector64<float>>(ref v);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector64<float> GetSizeSse(in Vector128<float> r)
    {
      var l = r;
      var h = Sse.MoveHighToLow(l,l);
      return Sse.Subtract(h,l).GetLower();
    }

    public static Vector64<float> GetSize(Vector128<float> r)
      => Sse.IsSupported
        ? GetSizeSse(r)
        : GetSizeNaive(r);

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
    public static explicit operator ValueTuple<Vector2,Vector2>(BoxF v)
      => Unsafe.As<BoxF, ValueTuple<Vector2,Vector2>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator BoxF(ValueTuple<Vector2,Vector2> v)
      => Unsafe.As<ValueTuple<Vector2,Vector2>, BoxF>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTuple<PointF,PointF>(BoxF v)
      => Unsafe.As<BoxF, ValueTuple<PointF,PointF>>(ref v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BoxF(ValueTuple<PointF,PointF> v)
      => Unsafe.As<ValueTuple<PointF,PointF>, BoxF>(ref v);

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