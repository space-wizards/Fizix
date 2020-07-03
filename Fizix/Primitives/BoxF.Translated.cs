using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BoxF TranslatedNaive(in BoxF r, Vector2 p)
      => new BoxF(r.X1 + p.X, r.Y1 + p.Y, r.X2 + p.X, r.Y2 + p.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BoxF TranslatedSse(in BoxF r, Vector128<float> p) {
      p = Sse.MoveLowToHigh(p, p);
      return Sse.Add(r, p);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoxF Translated(in BoxF r, Vector2 p)
      => Sse.IsSupported
        ? TranslatedSse(r, p.ToVector128())
        : TranslatedNaive(r, p);

  }

}