using System.Numerics;
using JetBrains.Annotations;

namespace Fizix {

  /// <summary>
  /// Extension methods for CardinalDirection enum.
  /// </summary>
  [PublicAPI]
  public static class CardinalDirectionExtensions {

    /// <summary>
    /// Converts a direction vector to the closest CardinalDirection enum.
    /// </summary>
    public static CardinalDirection GetCardinalDirection(this Vector2 vec)
      => new Angle(vec);

    /// <summary>
    /// Converts a CardinalDirection to an IntercardinalDirection enum.
    /// </summary>
    public static IntercardinalDirection ToIntercardinalDirection(this CardinalDirection dir)
      => (IntercardinalDirection) (2 * (int) dir);

  }

}