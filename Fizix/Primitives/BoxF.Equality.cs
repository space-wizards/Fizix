using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Fizix {

  public partial struct BoxF : IEquatable<BoxF>, IComparable<BoxF> {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(BoxF other)
      => _value.Equals(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(BoxF other)
      => Left < other.Left ? -1
        : Left > other.Left ? 1
        : Top < other.Top ? -1
        : Top > other.Top ? 1
        : Right < other.Right ? -1
        : Right > other.Right ? 1
        : Bottom < other.Bottom ? -1
        : Bottom > other.Bottom ? 1
        : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj)
      => obj is BoxF other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
      => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BoxF left, BoxF right)
      => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BoxF left, BoxF right)
      => !left.Equals(right);

  }

}