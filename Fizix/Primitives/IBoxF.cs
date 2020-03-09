using System.Runtime.CompilerServices;

namespace Fizix {

  public interface IBoxF {

    float Top {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    float Left {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    float Bottom {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    float Right {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    float Width {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    float Height {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    PointF Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    SizeF Size {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

  }

}