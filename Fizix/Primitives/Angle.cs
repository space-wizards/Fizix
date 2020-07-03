using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;
using Math = CannyFastMath.Math;
using MathF = CannyFastMath.MathF;

namespace Fizix {

  /// <summary>
  ///     A representation of an angle in radians.
  /// </summary>
  [PublicAPI]
  [Serializable]
  public readonly struct Angle : IEquatable<Angle> {

    public static Angle Zero => default;

    public const double East = 0;

    public const double NorthEast = Math.PI * (1 / 4.0);

    public const double North = Math.PI * (2 / 4.0);

    public const double NorthWest = Math.PI * (3 / 4.0);

    public const double West = Math.PI;

    public const double SouthWest = -Math.PI * (3 / 4.0);

    public const double South = -Math.PI * (2 / 4.0);

    public const double SouthEast = -Math.PI * (1 / 4.0);

    /// <summary>
    ///     Angle in radians.
    /// </summary>
    public readonly double Theta;

    /// <summary>
    ///     Angle in degrees.
    /// </summary>
    public double Degrees {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Math.Rad2Deg(Theta);
    }

    /// <summary>
    ///     Angle in turns.
    /// </summary>
    public double Turns {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Theta / Math.PI;
    }

    /// <summary>
    ///     Constructs an instance of an Angle.
    /// </summary>
    /// <param name="theta">The angle in radians.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle(double theta)
      => Theta = theta;

    /// <summary>
    ///     Constructs an instance of an angle from an (un)normalized direction vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle(Vector2 dir) {
      dir = dir.Normalized();
      Theta = Math.Atan2(dir.Y, dir.X);
    }

    private const double CardinalRadians = Math.TAU / 4; // Cut the circle into 4 pieces

    private const double CardinalOffset = CardinalRadians / 2; // offset the pieces by 1/2 their size

    private const double IntercardinalRadians = Math.TAU / 8; // Cut the circle into 8 pieces

    private const double IntercardinalOffset = IntercardinalRadians / 2; // offset the pieces by 1/2 their size

    /// <summary>
    ///     Rotates the vector counter-clockwise around its origin by the value of Theta.
    /// </summary>
    /// <param name="vec">Vector to rotate.</param>
    /// <returns>New rotated vector.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Rotate(in Vector2 vec) {
      var (x, y) = vec;
      Math.SinCos(Theta, out var sinTheta, out var cosTheta);
      var dx = cosTheta * x - sinTheta * y;
      var dy = sinTheta * x + cosTheta * y;
      return new Vector2((float) dx, (float) dy);
    }

    /// <summary>
    ///     Removes revolutions from a positive or negative angle to make it as small as possible.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle Normalized()
      => Normalize(Theta);

    /// <summary>
    ///     Removes revolutions from a positive or negative angle to make it as small as possible.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Normalize(double theta) {
      var r = Math.FusedMultiplyAdd(Math.Ceiling((Math.TAU * 2 - theta) * (1 / Math.TAU)), Math.TAU, theta);
      return Math.FusedMultiplyAdd(-Math.Truncate(r * (1 / Math.TAU)), Math.TAU, r);
    }

    /// <summary>
    ///     Removes revolutions from a positive or negative angle to make it as small as possible.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Angle Normalize(Angle angle)
      => Normalize(angle.Theta);

    // ReSharper disable CompareOfFloatsByEqualityOperator
    /// <inheritdoc />
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Angle other) {
      var t = Theta;
      var ot = other.Theta;
      if (t == ot) return true;

      var tn = Normalize(t);
      if (tn == ot) return true;

      var otn = Normalize(ot);
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (tn == otn) return true;
      if (t == otn) return true;

      return false;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(double other) {
      var t = Theta;
      var ot = other;
      if (t == ot) return true;

      var tn = Normalize(t);
      if (tn == ot) return true;

      var otn = Normalize(ot);
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (tn == otn) return true;
      //return t == otn;
      return false;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(float other) {
      var t = Theta;
      var ot = other;
      if (t == ot) return true;

      var tn = (float) Normalize(t);
      if (tn == ot) return true;

      var otn = (float) Normalize((double) ot);
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (tn == otn) return true;
      //return (float) t == otn;
      return false;
    }
    // ReSharper restore CompareOfFloatsByEqualityOperator

    /// <inheritdoc />
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;

      return obj is Angle angle && Equals(angle);
    }

    /// <inheritdoc />
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
      => Theta.GetHashCode();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Angle a, Angle b)
      => a.Equals(b);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Angle a, Angle b)
      => !(a == b);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle FlipPositive()
      => new Angle(FlipPositive(Theta));

    /// <summary>
    /// Calculates the congruent positive angle of a negative angle.
    /// Does nothing to a positive angle.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double FlipPositive(double theta)
      => Math.FusedMultiplyAdd(Math.One(theta >= 0), Math.TAU, theta);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Interpolate(double a, double b, double factor) {
      a = Normalize(a);
      b = Normalize(b);
      var delta = b - a;
      var wrappedDelta = Math.Clamp(delta - Math.Floor(delta / Math.TAU) * Math.TAU, 0.0, Math.TAU);
      wrappedDelta = Math.FusedMultiplyAdd(Math.Selector(wrappedDelta > Math.PI), Math.TAU, wrappedDelta);
      factor = Math.Clamp(factor, 0, 1);
      return new Angle(Math.FusedMultiplyAdd(wrappedDelta, factor, a));
    }

    /// <summary>
    ///     Linear interpolation with radial wrapping.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle Interpolate(Angle a, double b, double factor)
      => Interpolate((double) a, b, factor);

    /// <summary>
    ///     Linear interpolation with radial wrapping.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle Interpolate(double a, Angle b, double factor)
      => Interpolate(a, (double) b, factor);

    /// <summary>
    ///     Constructs a new angle, from degrees instead of radians.
    /// </summary>
    /// <param name="degrees">The angle in degrees.</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle FromDegrees(double degrees)
      => new Angle(Math.Deg2Rad(degrees));

    /// <summary>
    ///     Implicit conversion from Angle to <see cref="System.Double"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator double(Angle angle)
      => angle.Theta;

    /// <summary>
    ///     Explicit conversion from Angle to <see cref="System.Single"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator float(Angle angle)
      => (float) angle.Theta;

    /// <summary>
    ///     Implicit conversion from <see cref="System.Double"/>  to Angle.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Angle(double theta)
      => new Angle(theta);

    /// <summary>
    ///     Implicit conversion from <see cref="System.Single"/> to Angle.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Angle(float theta)
      => new Angle(theta);

    /// <summary>
    ///     Implicit conversion from <see cref="CardinalDirection"/> to Angle.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Angle(CardinalDirection dir) {
      var ang = CardinalRadians * (int) dir;

      ang = Math.FusedMultiplyAdd(Math.Selector(ang > Math.PI), Math.TAU, ang);

      return new Angle(ang);
    }

    /// <summary>
    ///     Implicit conversion from <see cref="IntercardinalDirection"/> to Angle.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Angle(IntercardinalDirection dir) {
      var ang = IntercardinalRadians * (int) dir;

      ang = Math.FusedMultiplyAdd(Math.Selector(ang > Math.PI), Math.TAU, ang);

      return new Angle(ang);
    }

    /// <summary>
    ///     Implicit conversion from Angle to <see cref="IntercardinalDirection"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IntercardinalDirection(Angle angle) {
      var ang = Normalize(angle.Theta);

      // convert -PI > PI to 0 > 2PI
      ang = Math.FusedMultiplyAdd(Math.One(ang < 0.0), Math.TAU, ang);

      return (IntercardinalDirection) (Math.Floor((ang + IntercardinalOffset) / IntercardinalRadians) % 8);
    }

    /// <summary>
    ///     Implicit conversion from Angle to <see cref="CardinalDirection"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CardinalDirection(Angle angle) {
      var ang = Normalize(angle.Theta);

      // convert -PI > PI to 0 > 2PI
      ang = Math.FusedMultiplyAdd(Math.One(ang < 0.0), Math.TAU, ang);

      return (CardinalDirection) (Math.Floor((ang + CardinalOffset) / CardinalRadians) % 4);
    }

    /// <summary>
    ///     Implicit conversion from Angle to <see cref="Vector2"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(Angle angle) {
      Math.SinCos(angle.Theta, out var y, out var x);
      return new Vector2((float) x, (float) y);
    }

    /// <summary>
    ///     Explicit conversion from <see cref="Vector2"/> to Angle.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Angle(Vector2 dir)
      => new Angle(dir);

    public override string ToString()
      => $"{Theta} rad";

  }

}