using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  [PublicAPI]
  public readonly struct CollisionBoxF<T> where T : struct {

    private readonly BoxF _box;

    public T CollisionMask {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get;
    }

    public CollisionBoxF(in BoxF box, T collisionMask) {
      _box = box;
      CollisionMask = collisionMask;
    }

    public float X1 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.X1;
    }

    public float Y1 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Y1;
    }

    public float X2 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.X2;
    }

    public float Y2 {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Y2;
    }

    public float Top {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Top;
    }

    public float Left {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Left;
    }

    public float Bottom {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Bottom;
    }

    public float Right {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Right;
    }

    public float Width {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Width;
    }

    public float Height {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Height;
    }

    public Vector2 TopLeft {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.TopLeft;
    }

    public Vector2 BottomRight {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.BottomRight;
    }

    public Vector2 Center {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Center;
    }

    public Vector2 Size {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _box.Size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
      => $"({Top}, {Left}, {Bottom}, {Right}) Mask: {CollisionMask}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BoxF(in CollisionBoxF<T> box)
      => Unsafe.As<CollisionBoxF<T>, BoxF>(ref Unsafe.AsRef(box));

  }

}