using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> TranslatedNaive(in BoxF r, PointF p) {
      var v = (r.X1 + p.X, r.Y1 + p.Y, r.X2 + p.X, r.Y2 + p.Y);
      return Unsafe.As<ValueTuple<float, float, float, float>, Vector128<float>>(ref v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> TranslatedSse(in BoxF r, PointF p) {
      p = Sse.MoveLowToHigh(p, p);
      return Sse.Add(r, p);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Translated(in BoxF r, PointF p)
      => Sse.IsSupported
        ? TranslatedSse(r, p)
        : TranslatedNaive(r, p);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> TranslatedNaive(in BoxF r, SizeF p) {
      var v = (r.X1 + p.Width, r.Y1 + p.Height, r.X2 + p.Width, r.Y2 + p.Height);
      return Unsafe.As<ValueTuple<float, float, float, float>, Vector128<float>>(ref v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> TranslatedSse(in BoxF r, SizeF p) {
      p = Sse.MoveLowToHigh(p, p);
      return Sse.Add(r, p);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Translated(in BoxF r, SizeF p)
      => Sse.IsSupported
        ? TranslatedSse(r, p)
        : TranslatedNaive(r, p);

  }

}