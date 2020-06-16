using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using CannyFastMath;
using MathF = System.MathF;

namespace Fizix {

  public readonly partial struct PointF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF NegateNaive(PointF v)
      => new PointF(-v.X, -v.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PointF NegateSse(PointF v) {
      var sign = Vector128.CreateScalarUnsafe(-0f);
      var x = Vector128.CreateScalarUnsafe(v.X);
      var y = Vector128.CreateScalarUnsafe(v.Y);
      var nx = Sse.Xor(sign, x);
      var ny = Sse.Xor(sign, y);
      return Sse.UnpackLow(nx, ny);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static PointF Negate(PointF v)
      => Sse.IsSupported ? NegateSse(v) : NegateNaive(v);

  }

}