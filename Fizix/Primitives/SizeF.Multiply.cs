using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct SizeF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF MultiplyNaive(SizeF a, SizeF b)
      => new SizeF(a.Width * b.Width, a.Height * b.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF MultiplySse(SizeF a, SizeF b)
      => Sse.Multiply(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Multiply(SizeF a, SizeF b)
      => (Vector2) (Sse.IsSupported ? MultiplySse(a, b) : MultiplyNaive(a, b));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF MultiplyNaive(SizeF a, float b)
      => new SizeF(a.Width * b, a.Height * b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF MultiplySse(SizeF a, float b)
      => Sse.Multiply(a, Vector128.Create(b));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 Multiply(SizeF a, float b)
      => (Vector2) (Sse.IsSupported ? MultiplySse(a, b) : MultiplyNaive(a, b));

  }

}