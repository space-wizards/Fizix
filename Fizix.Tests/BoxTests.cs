using System;
using System.Diagnostics;
using System.Security.Cryptography;
using NUnit.Framework;
using Fizix;

namespace Fizix.Tests {

  public class BoxTests {

    private static readonly BoxF[] Boxes1 = {
      (-1, -1, 1, 1), //2x2 square
      (-2, -2, 2, 2), //4x4 square
      (-3, 3, -3, 3), // point off to the bottom left
      (-3, -3, -3, -3), // point off to the top left
      (3, 3, 3, 3), // point off to the bottom right
      (3, -3, 3, -3), // point off to the top right
      (-1, -1, 1, 1), //2x2 square
      (-2, -2, 2, 2), //4x4 square
      (-1, -1, 1, 1), //2x2 square
      (-2, -2, 2, 2), //4x4 square
      (-1, -1, 1, 1), //2x2 square
      (-2, -2, 2, 2), //4x4 square
      (-1, -1, 1, 1), //2x2 square
      (-2, -2, 2, 2), //4x4 square
      (-3, -3, 3, 3), //6x6 square
      (-3, 3, -3, 3), // point off to the bottom left
      (-3, -3, -3, -3), // point off to the top left
      (3, 3, 3, 3), // point off to the bottom right
      (3, -3, 3, -3), // point off to the top right
    };

    private static readonly BoxF[] Boxes2 = {
      (-3, -3, 3, 3), //6x6 square
      (-1, -1, 1, 1), //2x2 square
      (-2, -2, 2, 2), //4x4 square
      (-3, 3, -3, 3), // point off to the bottom left
      (-3, -3, -3, -3), // point off to the top left
      (3, 3, 3, 3), // point off to the bottom right
      (3, -3, 3, -3), // point off to the top right
      (-3, 3, -3, 3), // point off to the bottom left
      (-3, -3, -3, -3), // point off to the top left
      (3, 3, 3, 3), // point off to the bottom right
      (3, -3, 3, -3), // point off to the top right
      (-3, 3, -3, 3), // point off to the bottom left
      (-3, -3, -3, -3), // point off to the top left
      (3, 3, 3, 3), // point off to the bottom right
      (3, -3, 3, -3), // point off to the top right
      (-2, -2, 2, 2), //4x4 square
      (-1, -1, 1, 1), //2x2 square
      (-2, -2, 2, 2), //4x4 square
      (-1, -1, 1, 1), //2x2 square
    };

    [Test]
    public void ContainsSanity() {
      Assert.True(BoxF.ContainsRectNaive(Boxes1[1], Boxes1[0]));
      Assert.True(BoxF.ContainsRectNaive(Boxes2[0], Boxes2[1]));
      Assert.False(BoxF.ContainsRectNaive(Boxes1[0], Boxes1[1]));
      Assert.False(BoxF.ContainsRectNaive(Boxes2[1], Boxes2[0]));
      for (var i = 0; i < 4; ++i) {
        Assert.True(BoxF.ContainsRectNaive(Boxes2[0], Boxes2[3 + i]));
        Assert.False(BoxF.ContainsRectNaive(Boxes2[3 + i], Boxes2[0]));
      }

      for (var i = 0; i < 4; ++i) {
        Assert.False(BoxF.ContainsRectNaive(Boxes2[1], Boxes2[3 + i]));
        Assert.False(BoxF.ContainsRectNaive(Boxes2[2], Boxes2[3 + i]));
      }
    }

    [Test]
    public void IntersectsSanity() {
      Assert.True(BoxF.IsIntersectingNaive(Boxes1[1], Boxes1[0]));
      Assert.True(BoxF.IsIntersectingNaive(Boxes2[0], Boxes2[1]));
      Assert.True(BoxF.IsIntersectingNaive(Boxes1[0], Boxes1[1]));
      Assert.True(BoxF.IsIntersectingNaive(Boxes2[1], Boxes2[0]));
      for (var i = 0; i < 4; ++i) {
        Assert.True(BoxF.IsIntersectingNaive(Boxes2[0], Boxes2[3 + i]));
        Assert.True(BoxF.IsIntersectingNaive(Boxes2[3 + i], Boxes2[0]));
      }

      for (var i = 0; i < 4; ++i) {
        Assert.False(BoxF.IsIntersectingNaive(Boxes2[1], Boxes2[3 + i]));
        Assert.False(BoxF.IsIntersectingNaive(Boxes2[2], Boxes2[3 + i]));
      }
    }

    [Test]
    public void UnionedSanity() {
      Assert.AreEqual(Boxes1[1], (BoxF) BoxF.UnionedNaive(Boxes1[1], Boxes1[0]));
      Assert.AreEqual(Boxes2[0], (BoxF) BoxF.UnionedNaive(Boxes2[0], Boxes2[1]));
      Assert.AreEqual(Boxes1[1], (BoxF) BoxF.UnionedNaive(Boxes1[0], Boxes1[1]));
      Assert.AreEqual(Boxes2[0], (BoxF) BoxF.UnionedNaive(Boxes2[1], Boxes2[0]));
      for (var i = 0; i < 4; ++i) {
        Assert.AreEqual(Boxes2[0], (BoxF) BoxF.UnionedNaive(Boxes2[0], Boxes2[3 + i]));
        Assert.AreEqual(Boxes2[0], (BoxF) BoxF.UnionedNaive(Boxes2[3 + i], Boxes2[0]));
      }
    }

    [Test]
    [TestCase(1000)]
    public unsafe void ContainsFuzz(int count) {
      var boxes = new BoxF[count];

      var rng = RandomNumberGenerator.Create();

      fixed (BoxF* pBoxes = &boxes[0])
        rng.GetBytes(new Span<byte>(pBoxes, count * sizeof(BoxF)));

      for (var i = 0; i < count; ++i)
        boxes[i].Normalize();

      for (var i = 0; i < count; ++i)
      for (var j = 0; j < count; ++j) {
        Assert.AreEqual(
          BoxF.ContainsRectNaive(boxes[i], boxes[j]),
          BoxF.ContainsRectSse(boxes[i], boxes[j]),
          $"Mismatch {boxes[i]} vs. {boxes[j]}"
        );
      }
    }

    [Test]
    [TestCase(1000)]
    public unsafe void IntersectsFuzz(int count) {
      var boxes = new BoxF[count];

      var rng = RandomNumberGenerator.Create();

      fixed (BoxF* pBoxes = &boxes[0])
        rng.GetBytes(new Span<byte>(pBoxes, count * sizeof(BoxF)));

      for (var i = 0; i < count; ++i)
        boxes[i].Normalize();

      for (var i = 0; i < count; ++i)
      for (var j = 0; j < count; ++j) {
        Assert.AreEqual(
          BoxF.IsIntersectingNaive(boxes[i], boxes[j]),
          BoxF.IsIntersectingSse(boxes[i], boxes[j]),
          $"Mismatch {boxes[i]} vs. {boxes[j]}"
        );
      }
    }

    [Test]
    [TestCase(1000)]
    public unsafe void UnionedFuzz(int count) {
      var boxes = new BoxF[count];

      var rng = RandomNumberGenerator.Create();

      fixed (BoxF* pBoxes = &boxes[0])
        rng.GetBytes(new Span<byte>(pBoxes, count * sizeof(BoxF)));

      for (var i = 0; i < count; ++i) {
        ref var box = ref boxes[i];
        box.Normalize();
        box = (
          float.IsNaN(box.X1) ? 0 : box.X1,
          float.IsNaN(box.Y1) ? 0 : box.Y1,
          float.IsNaN(box.X2) ? 0 : box.X2,
          float.IsNaN(box.Y2) ? 0 : box.Y2
        );
      }

      for (var i = 0; i < count; ++i)
      for (var j = 0; j < count; ++j) {
        Assert.AreEqual(
          BoxF.UnionedNaive(boxes[i], boxes[j]),
          BoxF.UnionedSse(boxes[i], boxes[j]),
          $"Mismatch {boxes[i]} vs. {boxes[j]}"
        );
      }
    }

    [Test]
    [TestCase(10000)]
    public unsafe void IntersectsBench(int count) {
      var boxes = new BoxF[count];

      var rng = RandomNumberGenerator.Create();

      fixed (BoxF* pBoxes = &boxes[0])
        rng.GetBytes(new Span<byte>(pBoxes, count * sizeof(BoxF)));

      for (var i = 0; i < count; ++i)
        boxes[i].Normalize();

      var sw = Stopwatch.StartNew();

      for (var run = 0; run < 3; ++run) {
        sw.Restart();
        for (var i = 0; i < count; ++i)
        for (var j = 0; j < count; ++j)
          BoxF.IsIntersectingNaive(boxes[i], boxes[j]);

        var t = sw.ElapsedTicks;
        Console.WriteLine($"IsIntersectingNaive ticks {run}: {t} total, ~{t/(double)(count*count)} ea.");
      }

      for (var run = 0; run < 3; ++run) {
        sw.Restart();
        for (var i = 0; i < count; ++i)
        for (var j = 0; j < count; ++j)
          BoxF.IsIntersectingSse(boxes[i], boxes[j]);

        var t = sw.ElapsedTicks;
        Console.WriteLine($"IsIntersectingSse ticks {run}: {t}, ~{t/(double)(count*count)} ea.");
      }
    }

    [Test]
    [TestCase(10000)]
    public unsafe void ContainsBench(int count) {
      var boxes = new BoxF[count];

      var rng = RandomNumberGenerator.Create();

      fixed (BoxF* pBoxes = &boxes[0])
        rng.GetBytes(new Span<byte>(pBoxes, count * sizeof(BoxF)));

      for (var i = 0; i < count; ++i)
        boxes[i].Normalize();

      var sw = Stopwatch.StartNew();

      for (var run = 0; run < 3; ++run) {
        sw.Restart();
        for (var i = 0; i < count; ++i)
        for (var j = 0; j < count; ++j)
          BoxF.ContainsRectNaive(boxes[i], boxes[j]);

        var t = sw.ElapsedTicks;
        Console.WriteLine($"ContainsRectNaive ticks {run}: {t}, ~{t/(double)(count*count)} ea.");
      }

      for (var run = 0; run < 3; ++run) {
        sw.Restart();
        for (var i = 0; i < count; ++i)
        for (var j = 0; j < count; ++j)
          BoxF.ContainsRectSse(boxes[i], boxes[j]);

        var t = sw.ElapsedTicks;
        Console.WriteLine($"ContainsRectSse ticks {run}: {t}, ~{t/(double)(count*count)} ea.");
      }
    }

    [Test]
    [TestCase(10000)]
    public unsafe void UnionedBench(int count) {
      var boxes = new BoxF[count];

      var rng = RandomNumberGenerator.Create();

      fixed (BoxF* pBoxes = &boxes[0])
        rng.GetBytes(new Span<byte>(pBoxes, count * sizeof(BoxF)));

      for (var i = 0; i < count; ++i)
        boxes[i].Normalize();

      var sw = Stopwatch.StartNew();

      for (var run = 0; run < 3; ++run) {
        sw.Restart();
        for (var i = 0; i < count; ++i)
        for (var j = 0; j < count; ++j)
          BoxF.UnionedNaive(boxes[i], boxes[j]);

        var t = sw.ElapsedTicks;
        Console.WriteLine($"UnionedNaive ticks {run}: {t}, ~{t/(double)(count*count)} ea.");
      }

      for (var run = 0; run < 3; ++run) {
        sw.Restart();
        for (var i = 0; i < count; ++i)
        for (var j = 0; j < count; ++j)
          BoxF.UnionedSse(boxes[i], boxes[j]);

        var t = sw.ElapsedTicks;
        Console.WriteLine($"UnionedSse ticks {run}: {t}, ~{t/(double)(count*count)} ea.");
      }
    }

  }

}