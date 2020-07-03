using System.Collections.Generic;
using System.Numerics;
using CannyFastMath;
using NUnit.Framework;

namespace Fizix.Tests {

  [Parallelizable(ParallelScope.All | ParallelScope.Fixtures)]
  [TestFixture]
  [TestOf(typeof(Angle))]
  public class AngleTests {

    private static IEnumerable<(float, float, IntercardinalDirection, double)> Intercardinals => new (float, float, IntercardinalDirection, double)[] {
      (1, 0, IntercardinalDirection.East, Angle.East),
      (1, 1, IntercardinalDirection.NorthEast, Angle.NorthEast),
      (0, 1, IntercardinalDirection.North, Angle.North),
      (-1, 1, IntercardinalDirection.NorthWest, Angle.NorthWest),
      (-1, 0, IntercardinalDirection.West, Angle.West),
      (-1, -1, IntercardinalDirection.SouthWest, Angle.SouthWest),
      (0, -1, IntercardinalDirection.South, Angle.South),
      (1, -1, IntercardinalDirection.SouthEast, Angle.SouthEast),
      (1.5f, 0, IntercardinalDirection.East, Angle.East),
      (0.5f, 0, IntercardinalDirection.East, Angle.East),
    };

    private static IEnumerable<(float, float, CardinalDirection)> Cardinals => new (float, float, CardinalDirection)[] {
      (1, 0, CardinalDirection.East),
      (0, 1, CardinalDirection.North),
      (-1, 0, CardinalDirection.West),
      (0, -1, CardinalDirection.South),
      (1.5f, 0, CardinalDirection.East),
      (0.5f, 0, CardinalDirection.East),
    };

    [Test]
    public void TestAngleZero() {
      Assert.That(Angle.Zero.Theta, Is.EqualTo(0));
      Assert.That(Angle.Zero.Degrees, Is.EqualTo(0));
    }

    [Test]
    public void TestAngleNormalized([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test)
      => Assert.Multiple(() => {
        var control = new Angle(new Vector2(test.Item1, test.Item2));

        var target = control.Normalized();
        Assert.That(target, Is.EqualTo(control), "Target vs. control");

        var targetPlusRev = new Angle(target + Math.TAU).Normalized();
        Assert.That((double) targetPlusRev, Is.LessThan(Math.TAU));
        Assert.That((double) targetPlusRev, Is.GreaterThan(-Math.TAU));
        var controlPlusRev = new Angle(control + Math.TAU).Normalized();
        Assert.That((double) controlPlusRev, Is.LessThan(Math.TAU));
        Assert.That((double) controlPlusRev, Is.GreaterThan(-Math.TAU));
        Assert.That(targetPlusRev.Equals(controlPlusRev),
          () => $"(+ revolution) Target vs. control:\n{targetPlusRev} vs. {controlPlusRev}");

        var targetMinusRev = new Angle(target - Math.TAU).Normalized();
        Assert.That((double) targetPlusRev, Is.LessThan(Math.TAU));
        Assert.That((double) targetPlusRev, Is.GreaterThan(-Math.TAU));
        var controlMinusRev = new Angle(control - Math.TAU).Normalized();
        Assert.That((double) controlPlusRev, Is.LessThan(Math.TAU));
        Assert.That((double) controlPlusRev, Is.GreaterThan(-Math.TAU));
        Assert.That(targetMinusRev.Equals(controlMinusRev),
          () => $"(- revolution) Target vs. control:\n{targetMinusRev} vs. {controlMinusRev}");
      });

    [Test]
    public void TestAngleEquals([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test)
      => Assert.Multiple(() => {
        var control = new Angle(new Vector2(test.Item1, test.Item2));

        var target = new Angle(test.Item4);
        Assert.That(target, Is.EqualTo(control), "Target vs. control");

        var targetPlusRev = new Angle(target + Math.TAU);
        Assert.That((double) targetPlusRev.Normalized(), Is.EqualTo((double) control.Normalized()), "Target + revolution vs. control");

        var targetMinusRev = new Angle(target - Math.TAU);
        Assert.That((double) targetMinusRev.Normalized(), Is.EqualTo((double) control.Normalized()), "Target - revolution vs. control");
      });

    [Test]
    public void TestAngleFromDegrees([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test) {
      var rads = test.Item4;
      var degrees = Math.Rad2Deg(rads);

      var target = Angle.FromDegrees(degrees);
      var control = new Angle(rads);

      Assert.That(target, Is.EqualTo(control));
    }

    [Test]
    public void TestAngleToDoubleImplicitConversion([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test) {
      var control = new Angle(new Vector2(test.Item1, test.Item2));

      double impl = control;
      var expl = (double) control;

      Assert.That(impl, Is.EqualTo(expl));
    }

    [Test]
    public void TestDoubleToAngleImplicitConversion([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test) {
      var rads = test.Item4;

      Angle impl = rads;
      var expl = new Angle(rads);

      Assert.That(impl, Is.EqualTo(expl));
    }

    [Test]
    public void TestFloatToAngleImplicitConversion([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test) {
      var rads = (float) test.Item4;

      Angle impl = rads;
      var expl = new Angle(rads);

      Assert.That(impl, Is.EqualTo(expl));
    }

    [Test]
    [Sequential]
    public void TestAngleToVector2([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test) {
      const double error = 1.5e-32;

      var control = new Vector2(test.Item1, test.Item2).Normalized();
      var target = new Angle(test.Item4);

      Assert.That((control - target).LengthSquared, Is.AtMost(error));
    }

    [Test]
    [Sequential]
    public void TestAngleToIntercardinalDirection([ValueSource(nameof(Intercardinals))] (float, float, IntercardinalDirection, double) test) {
      var target = new Angle(test.Item4);

      Assert.That((IntercardinalDirection) target, Is.EqualTo(test.Item3));
    }

    [Test]
    [Sequential]
    public void TestAngleToCardinal([ValueSource(nameof(Cardinals))] (float, float, CardinalDirection) test) {
      var target = (Angle) new Vector2(test.Item1, test.Item2);

      Assert.That((CardinalDirection) target, Is.EqualTo(test.Item3));
    }

    [Test]
    public void TestAngleRotateVec() {
      var angle = new Angle(Math.PI / 6);
      var vec = new Vector2(0.5f, 0.5f);

      var result = angle.Rotate(vec);

      var expected = new Vector2(0.18301271f, 0.6830127f);
      Assert.That(result, Is.EqualTo(expected));
    }

  }

}