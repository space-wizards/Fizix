using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {
  [PublicAPI]
  public static class QuadFExtensions {
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this QuadF q, Vector2 p)
      => QuadF.ContainsPoint(q, p);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this QuadF q, in BoxF b)
      => QuadF.ContainsBox(q, b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this QuadF q, in QuadF other)
      => QuadF.ContainsQuad(q, other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetCorners(this QuadF q, out Vector2 tl, out Vector2 br, out Vector2 tr, out Vector2 bl)
      => QuadF.Corners(q, out tl, out br, out tr, out bl);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetBounds(this QuadF q, out BoxF b)
      => QuadF.BoundingBox(q, out b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(this QuadF q, in BoxF b)
      => QuadF.IntersectsBox(q, b);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(this QuadF q, in QuadF other)
      => QuadF.IntersectsQuad(q, other);

  }

}