/*
 * Initially based on Box2D by Erin Catto, license follows;
 *
 * Copyright (c) 2009 Erin Catto http://www.box2d.org
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software
 * in a product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System.Runtime.CompilerServices;

namespace Fizix {

  public partial class BoxTree {

    protected struct Branch : INode {

      public BoxF Box;

      BoxF INode.Box {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Box;
      }

      public Proxy Parent;

      Proxy INode.Parent {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Parent;
      }

      public Proxy Child1, Child2;

      private bool _used;

      public bool IsFree {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !_used;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _used = !value;
      }

      public bool IsLeaf {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
      }

      public int Height;

      public override string ToString()
        => $@"Parent: {(Parent.IsFree ? "None" : Parent.ToString())}, {
          (IsFree
            ? "Free"
            : $"children: {Child1} and {Child2}")}";

    }

  }

}