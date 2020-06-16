using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public static class BoxFExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this BoxF r, PointF p)
      => BoxF.ContainsPoint(r, p);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this UiBoxF r, PointF p)
      => BoxF.ContainsPoint((BoxF) r, p);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this BoxF r, in BoxF o)
      => BoxF.ContainsRect(r, o);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(in this UiBoxF r, in BoxF o)
      => BoxF.ContainsRect((BoxF) r, o);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(in this BoxF a, in BoxF b)
      => BoxF.IsIntersecting(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(in this UiBoxF a, in BoxF b)
      => BoxF.IsIntersecting(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoxF Translate(ref this BoxF r, PointF p) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (p == default) return r;

      return BoxF.Translated(r, p);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UiBoxF Translate(ref this UiBoxF r, PointF p) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (p == default) return r;

      return BoxF.Translated((BoxF) r, p);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoxF Translate(ref this BoxF r, SizeF p) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (p == default) return r;

      return BoxF.Translated(r, p);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UiBoxF Translate(ref this UiBoxF r, SizeF p) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (p == default) return r;

      return BoxF.Translated((BoxF) r, p);
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
    public static BoxF Union(this BoxF r, in BoxF o)
      => BoxF.Unioned(r, o);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UiBoxF Union(this UiBoxF r, in UiBoxF o)
      => BoxF.Unioned(r, o);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Perimeter(this BoxF r)
      => BoxF.Perimeter(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Perimeter(this UiBoxF r)
      => BoxF.Perimeter(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Area(this BoxF r)
      => BoxF.Area(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Area(this UiBoxF r)
      => BoxF.Area(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasNaN(this BoxF r)
      => BoxF.ContainsNaN(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasNaN(this UiBoxF r)
      => BoxF.ContainsNaN(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(this BoxF r)
      => BoxF.Finite(r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(this UiBoxF r)
      => BoxF.Finite(Unsafe.As<UiBoxF, BoxF>(ref Unsafe.AsRef(r)));

  }

}