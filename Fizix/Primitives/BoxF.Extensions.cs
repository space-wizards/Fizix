using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public static class BoxFExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this BoxF r, in PointF p)
      => BoxF.ContainsPoint(r, p);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this UiBoxF r, in PointF p)
      => BoxF.ContainsPoint(r, p);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this BoxF r, in BoxF o)
      => BoxF.ContainsRect(r, o);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this UiBoxF r, in BoxF o)
      => BoxF.ContainsRect(r, o);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(in this BoxF a, in BoxF b)
      => BoxF.IsIntersecting(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(in this UiBoxF a, in BoxF b)
      => BoxF.IsIntersecting(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref BoxF Translate(ref this BoxF r, in PointF p) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (p == default) return ref r;
      r = BoxF.Translated(r, p);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref UiBoxF Translate(ref this UiBoxF r, in PointF p) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (p == default) return ref r;
      r = BoxF.Translated(r, p);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref BoxF Scale(ref this BoxF r, float f) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (f == 1f) return ref r;
      r = BoxF.Scaled(r, f);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref UiBoxF Scale(ref this UiBoxF r, float f) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (f == 1f) return ref r;
      r = BoxF.Scaled(r, f);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref BoxF Grow(ref this BoxF r, float f) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (f == 0f) return ref r;
      r = BoxF.Grown(r, f);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref UiBoxF Grow(ref this UiBoxF r, float f) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (f == 0f) return ref r;
      r = BoxF.Grown(r, f);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref BoxF Normalize(ref this BoxF r) {
      r = BoxF.Normalized(r);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref UiBoxF Normalize(ref this UiBoxF r) {
      r = BoxF.Normalized(r);
      return ref r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoxF Union(in this BoxF r, in BoxF o)
      => BoxF.Unioned(r, o);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UiBoxF Union(in this UiBoxF r, in UiBoxF o)
      => BoxF.Unioned(r, o);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Perimeter(in this BoxF r)
      => BoxF.Perimeter(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Perimeter(in this UiBoxF r)
      => BoxF.Perimeter(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Area(in this BoxF r)
      => BoxF.Area(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Area(in this UiBoxF r)
      => BoxF.Area(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasNaN(in this BoxF r)
      => BoxF.ContainsNaN(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasNaN(in this UiBoxF r)
      => BoxF.ContainsNaN(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(in this BoxF r)
      => BoxF.Finite(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(in this UiBoxF r)
      => BoxF.Finite(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

  }

}