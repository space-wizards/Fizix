using System.Numerics;
using JetBrains.Annotations;

namespace Fizix {

  /// <summary>
  /// Extension methods for IntercardinalDirection enum.
  /// </summary>
  [PublicAPI]
  public static class IntercardinalDirectionExtensions {

    /// <summary>
    /// Converts a direction vector to the closest IntercardinalDirection enum.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static IntercardinalDirection GetIntercardinalDirection(this Vector2 vec)
      => new Angle(vec);

    /// <summary>
    /// Truncates a IntercardinalDirection to the closest CardinalDirection enum.
    /// </summary>
    public static CardinalDirection ToCardinalDirection(this IntercardinalDirection dir)
      => (CardinalDirection) ((int) dir / 2);

  }

}