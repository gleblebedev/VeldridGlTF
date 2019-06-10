﻿using System.Numerics;

namespace VeldridGlTF.Viewer
{
    public struct ClipPlaneInfo
    {
        public Vector4 ClipPlane;
        public int Enabled;

        public ClipPlaneInfo(Plane clipPlane, bool enabled)
        {
            ClipPlane = new Vector4(clipPlane.Normal, clipPlane.D);
            Enabled = enabled ? 1 : 0;
        }
    }
}
