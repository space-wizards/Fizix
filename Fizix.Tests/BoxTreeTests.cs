using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using NUnit.Framework;
using Fizix;

namespace Fizix.Tests {

  [TestFixture]
  [TestOf(typeof(BoxTree<>))]
  public class BoxTreeTests {

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
    public void AddAndGrow() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      var initBrCap = dt.BranchCapacity;

      Assert.AreEqual(16, initBrCap);

      var initLfCap = dt.LeafCapacity;

      Assert.AreEqual(16, initLfCap);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
          Assert.True(dt.Any(x => x == i), $"After Added Enum Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
          Assert.True(dt.Any(x => x == i), $"After All Added Enum Contains {i}");
        }
      });

      Assert.That(dt.BranchCapacity, Is.AtLeast(initBrCap));
      Assert.That(dt.LeafCapacity, Is.AtLeast(initLfCap));
    }

    [Test]
    public void AddDuplicates() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.False(dt.Add(i), $"Add Dupe {i}");
          Assert.True(dt.Contains(i), $"After Dupe Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Contains(i), $"After All Dupes Added Contains {i}");
      });
    }

    [Test]
    public void RemoveMissingWhileEmpty() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.False(dt.Remove(i), $"Remove {i}");
      });
    }

    [Test]
    public void UpdateMissingWhileEmpty() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.False(dt.Update(i), $"Update {i}");
      });
    }

    [Test]
    public void RemoveMissingNotEmpty() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
      });

      Assert.Multiple(() => {
        for (var i = Boxes1.Length; i < Boxes1.Length + Boxes2.Length; ++i) {
          Assert.False(dt.Remove(i), $"Remove {i}");
          Assert.False(dt.Contains(i), $"After Removed Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = Boxes1.Length; i < Boxes1.Length + Boxes2.Length; ++i)
          Assert.False(dt.Contains(i), $"After All Removed Contains {i}");
      });
    }

    [Test]
    public void UpdateMissingNotEmpty() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
      });

      Assert.Multiple(() => {
        for (var i = Boxes1.Length; i < Boxes1.Length + Boxes2.Length; ++i)
          Assert.False(dt.Update(i), $"Update {i}");
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Contains(i), $"After All Updated Contains {i}");
      });
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    [TestCase(10)]
    [TestCase(11)]
    [TestCase(12)]
    [TestCase(13)]
    [TestCase(14)]
    [TestCase(15)]
    [TestCase(16)]
    [TestCase(17)]
    [TestCase(18)]
    public void AddThenUpdateN(int n) {
      var boxes = Boxes1;
      var dt = new BoxTree<int>((in int x) => boxes[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
          Assert.True(dt.Any(x => x == i), $"After All Added Enum Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
          Assert.True(dt.Any(x => x == i), $"After All Added Enum Contains {i}");
        }
      });

      boxes = Boxes2;
      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          if (Boxes1[i].Contains(Boxes2[i]))
            Assert.False(dt.Update(i), $"Update {i}");
          else
            Assert.True(dt.Update(i), $"Update {i}");
          Assert.True(dt.Contains(i), $"After Update Contains {i}");
          Assert.True(dt.Any(x => x == i), $"After Update Enum Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          Assert.True(dt.Contains(i), $"After All Updated Contains {i}");
          Assert.True(dt.Any(x => x == i), $"After All Updated Enum Contains {i}");
        }
      });
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    [TestCase(10)]
    [TestCase(11)]
    [TestCase(12)]
    [TestCase(13)]
    [TestCase(14)]
    [TestCase(15)]
    [TestCase(16)]
    [TestCase(17)]
    [TestCase(18)]
    public void AddThenRemoveN(int n) {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          Assert.True(dt.Remove(i), $"Remove {i}");
          Assert.False(dt.Contains(i), $"After Removed Contains {i}");
        }
      });
    }

    [Test]
    public void AddThenRemove() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Remove(i), $"Remove {i}");
          Assert.False(dt.Contains(i), $"After Removed Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.False(dt.Contains(i), $"After All Removed Contains {i}");
      });
    }

    [Test]
    public void AddThenRemoveInReverse() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i) {
          Assert.True(dt.Remove(i), $"Remove {i}");
          Assert.False(dt.Contains(i), $"After Removed Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.False(dt.Contains(i), $"After All Removed Contains {i}");
      });
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    [TestCase(10)]
    [TestCase(11)]
    [TestCase(12)]
    [TestCase(13)]
    [TestCase(14)]
    [TestCase(15)]
    [TestCase(16)]
    [TestCase(17)]
    [TestCase(18)]
    public void AddThenUpdateThenRemoveN(int n) {
      var boxes = Boxes1;
      var dt = new BoxTree<int>((in int x) => boxes[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          Assert.True(dt.Add(i), $"Add {i}");
          Assert.True(dt.Contains(i), $"After Add Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i)
          Assert.True(dt.Contains(i), $"After All Added Contains {i}");
      });
      boxes = Boxes2;

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i) {
          if (Boxes1[i].Contains(Boxes2[i]))
            Assert.False(dt.Update(i), $"Update {i}");
          else
            Assert.True(dt.Update(i), $"Update {i}");
          Assert.True(dt.Contains(i), $"After Update Contains {i}");
        }
      });

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i)
          Assert.True(dt.Contains(i), $"After All Updated Contains {i}");
      });

      Assert.Multiple(() => {
        for (var i = 0; i < n; ++i)
          Assert.True(dt.Remove(i), $"Remove {i}");
      });
    }

    [Test]
    public void AddAndQuery() {
      var dt = new BoxTree<int>((in int x) => Boxes1[x], capacity: 16, growthFunc: x => x += 2);

      Assert.Multiple(() => {
        for (var i = 0; i < Boxes1.Length; ++i)
          Assert.True(dt.Add(i), $"Add {i}");
      });

      var point = new PointF(0, 0);

      var containers = Enumerable.Range(0, Boxes1.Length)
        .Where(x => Boxes1[x].Contains(point))
        .OrderBy(x => x).ToArray();

      var results = dt.Query(point)
        .OrderBy(x => x).ToArray();

      Assert.Multiple(() => {
        Assert.AreEqual(containers.Length, results.Length, "Length");
        var l = Math.Min(containers.Length, results.Length);
        for (var i = 0; i < l; ++i)
          Assert.AreEqual(containers[i], results[i]);
      });
    }

    [Test]
    [TestCase(500)]
    [TestCase(5000)]
    [TestCase(50000)]
    public unsafe void BigAddAndQuery(int count) {
#if DEBUG
      if (count > 1000)
        Assert.Inconclusive("This would take too long under debug conditions due to validation.");
#endif
      var boxes = new BoxF[count];

      var rng = RandomNumberGenerator.Create();

      fixed (BoxF* pBoxes = &boxes[0])
        rng.GetBytes(new Span<byte>(pBoxes, count * sizeof(BoxF)));

      for (var i = 0; i < count; ++i)
        boxes[i].Normalize();

      var dt = new BoxTree<int>((in int x) => boxes[x], capacity: count / 2, growthFunc: x => x + count / 2);

      Assert.Multiple(() => {
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < boxes.Length; ++i)
          Assert.True(dt.Add(i), $"Add {i}");
        TestContext.Out.WriteLine($"Added {count} in {sw.ElapsedMilliseconds}ms");
      });

      PointF point = (0, 0);

      var containers = Enumerable.Range(0, boxes.Length)
        .Where(x => boxes[x].Contains(point))
        .Distinct()
        .OrderBy(x => x).ToArray();

      var results = new List<int>(containers.Length);
      {
        var sw = Stopwatch.StartNew();
        foreach (var item in dt.Query(point)) {
          results.Add(item);
        }

        TestContext.Out.WriteLine($"Queried {results.Count} in {sw.ElapsedMilliseconds}ms");
      }
      results.Sort(Comparer<int>.Default);

      //Assert.Multiple(() =>
      {
        Assert.AreEqual(containers.Length, results.Count, "Length");
        var l = Math.Min(containers.Length, results.Count);
        for (var i = 0; i < l; ++i)
          Assert.AreEqual(containers[i], results[i]);
      }
      //);
      {
        Assert.AreEqual(containers.Length, results.Count, "Length");
        var l = Math.Min(containers.Length, results.Count);
        for (var i = 0; i < l; ++i)
          Assert.AreEqual(containers[i], results[i]);
      }

#if DEBUG
      //var debugView = dt.DebugAllocatedNodes;
#endif
    }

    // TODO: other BoxF, Ray, Point query method tests

  }

}