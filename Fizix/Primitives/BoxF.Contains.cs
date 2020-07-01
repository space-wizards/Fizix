using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Fizix {

  public readonly partial struct BoxF {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsRectNaive(in BoxF a, in BoxF b)
      => b.Left >= a.Left
        && b.Right <= a.Right
        && b.Top >= a.Top
        && b.Bottom <= a.Bottom;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsRectSse(in BoxF a, in BoxF b) {
      var aMin = Sse.MoveLowToHigh(a, a);
      var aMax = Sse.MoveHighToLow(a, a);
      var bMin = Sse.MoveLowToHigh(b, b);
      var bMax = Sse.MoveHighToLow(b, b);
      var gt = Sse.CompareGreaterThan(aMin, bMin);
      var lt = Sse.CompareLessThan(aMax, bMax);
      var oob = Sse.Or(gt, lt);
      return Sse.MoveMask(oob) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsRect(in BoxF a, in BoxF b)
      => Sse.IsSupported
        ? ContainsRectSse(a, b)
        : ContainsRectNaive(a, b);

  }

}