using System;
using System.Runtime.CompilerServices;

namespace Fizix {

  public sealed partial class BoxTree<T> {

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public void RebuildOptimal(int free = 0) {
      Validate();
      throw new NotImplementedException();
    }

  }

}