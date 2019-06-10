﻿using System.Numerics;
using Veldrid.Utilities;

namespace VeldridGlTF.Viewer.Objects
{
    internal class MirrorMesh
    {
        public static Plane Plane { get; set; } = new Plane(Vector3.UnitY, 0);
    }
}
