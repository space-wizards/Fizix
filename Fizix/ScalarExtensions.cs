using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public static class Scalar {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExchangeWith<T>(ref this T a, ref T b) where T : struct {
      var t = a;
      a = b;
      b = t;
    }

  }

}