using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fizix {

  // TODO: not implemented
  [PublicAPI]
  public struct CollisionManifold {

    public ContactPoint ContactA, ContactB;

    public Vector2 Center;

    public Vector2 Normal;

    public bool AlongFace;

    public bool OnCorner {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => !AlongFace;
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => AlongFace = !value;
    }

    public CollisionManifold(ContactPoint a, ContactPoint b, Vector2 normal, bool alongFace) {
      ContactA = a;
      ContactB = b;
      Center = (ContactA.Point + ContactB.Point) * .5f;
      Normal = normal;
      AlongFace = alongFace;
    }

  }

  [PublicAPI]
  public struct ContactPoint {

    public Vector2 Point;

    public float NormalImpulse;

    public float TangentImpulse;

  }

}