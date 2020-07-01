using System;
using System.Runtime.CompilerServices;

namespace Fizix {

  internal static class ProxyExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once RedundantAssignment
    public static void Free(ref this BoxTree.Proxy proxy)
      => BoxTree.Proxy.SetFree(out proxy);

  }

  public partial class BoxTree {

    protected internal readonly struct Proxy : IEquatable<Proxy>, IComparable<Proxy> {

      private const uint LeafBitMask = 0x80000000u;

      private readonly uint _value;

      private static Proxy Free {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new Proxy(uint.MaxValue);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal static void SetFree(out Proxy proxy) => proxy = Free;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Proxy(uint v) => _value = v;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Proxy(int v, bool isLeaf) => _value = isLeaf ? LeafBitMask | (uint) v : (uint) v;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Proxy(uint v, bool isLeaf) => _value = isLeaf ? LeafBitMask | v : v;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Equals(Proxy other)
        => _value == other._value;

      public bool IsFree {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this == Free;
      }

      public bool IsLeaf {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_value & LeafBitMask) != 0;
      }

      public int LeafIndex {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int) (_value & ~ LeafBitMask);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int CompareTo(Proxy other)
        => _value.CompareTo(other._value);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override bool Equals(object? obj)
        => obj is Proxy other && Equals(other);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override int GetHashCode() => (int) _value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static explicit operator uint(Proxy n) => n._value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static explicit operator Proxy(uint v) => new Proxy(v);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static explicit operator Proxy(int v) => new Proxy((uint) v);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool operator ==(Proxy a, Proxy b) => a._value == b._value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool operator !=(Proxy a, Proxy b) => a._value != b._value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool operator >(Proxy a, Proxy b) => a._value > b._value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool operator <(Proxy a, Proxy b) => a._value < b._value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool operator >=(Proxy a, Proxy b) => a._value >= b._value;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool operator <=(Proxy a, Proxy b) => a._value <= b._value;

      public override string ToString()
        => IsFree ? "Free" : IsLeaf ? $"Leaf {LeafIndex}" : $"Branch: {_value}";

    }

  }

}