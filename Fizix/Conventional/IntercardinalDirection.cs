using System;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  [Serializable]
  public enum IntercardinalDirection {

    East = 0,

    NorthEast,

    North,

    NorthWest,

    West,

    SouthWest,

    South,

    SouthEast

  }

}