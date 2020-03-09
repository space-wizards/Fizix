using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> TranslatedNaive(in Vector128<float> vr, in Vector64<float> vp) {
      ref var r = ref Unsafe.As<Vector128<float>, BoxF>(ref Unsafe.AsRef(vr));
      ref var p = ref Unsafe.As<Vector64<float>, PointF>(ref Unsafe.AsRef(vp));
      var v = (r.X1 + p.X, r.Y1 + p.Y, r.X2 + p.X, r.Y2 + p.Y);
      return Unsafe.As<ValueTuple<float, float, float, float>, Vector128<float>>(ref v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<float> TranslatedSse(in Vector128<float> r, in Vector64<float> p) {
      var sizeVec = Unsafe.As<Vector64<float>, Vector128<float>>(ref Unsafe.AsRef(p));
      sizeVec = Sse.MoveLowToHigh(sizeVec, sizeVec);
      return Sse.Add(r, sizeVec);
    }
    
    public static Vector128<float> Translated(in Vector128<float> r, in Vector64<float> p)
      => Sse.IsSupported
        ? TranslatedSse(r, p)
        : TranslatedNaive(r, p);

  }

}