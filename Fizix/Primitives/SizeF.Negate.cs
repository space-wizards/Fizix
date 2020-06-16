using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using CannyFastMath;
using MathF = System.MathF;

namespace Fizix {

  public readonly partial struct SizeF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF NegateNaive(SizeF v)
      => new SizeF(-v.Width, -v.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SizeF NegateSse(SizeF v) {
      var sign = Vector128.CreateScalarUnsafe(-0f);
      var x = Vector128.CreateScalarUnsafe(v.Width);
      var y = Vector128.CreateScalarUnsafe(v.Height);
      var nx = Sse.Xor(sign, x);
      var ny = Sse.Xor(sign, y);
      return Sse.UnpackLow(nx, ny);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static SizeF Negate(SizeF v)
      => Sse.IsSupported ? NegateSse(v) : NegateNaive(v);

  }

}