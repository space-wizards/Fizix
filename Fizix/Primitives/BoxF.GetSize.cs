using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF GetSizeNaive(in BoxF r)
      => new SizeF(r.Width, r.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF GetSizeSse(Vector128<float> r) {
      var l = r;
      var h = Sse.MoveHighToLow(l, l);
      return Sse.Subtract(h, l);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF GetSize(BoxF r)
      => Sse.IsSupported
        ? GetSizeSse(r)
        : GetSizeNaive(r);

  }

}