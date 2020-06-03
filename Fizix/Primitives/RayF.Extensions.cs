using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public static class RayExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(in this RayF ray, in BoxF box, out float distance, out PointF location, float epsilon = 1e-7f)
      => RayF.IntersectsBox(ray, box, out distance, out location, epsilon);

  }

}