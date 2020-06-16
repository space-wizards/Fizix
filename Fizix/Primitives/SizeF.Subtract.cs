using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct SizeF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF SubtractNaive(SizeF a, SizeF b)
      => new SizeF(a.Width - b.Width, a.Height - b.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF SubtractSse(SizeF a, SizeF b)
      => Sse.Subtract(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Subtract(SizeF a, SizeF b)
      => (Vector2) (Sse.IsSupported ? SubtractSse(a, b) : SubtractNaive(a, b));

  }

}