﻿using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer
{
    internal class RenderOrderKeyComparer : IComparer<Renderable>
    {
        public Vector3 CameraPosition { get; set; }
        public int Compare(Renderable x, Renderable y)
        {
            return x.GetRenderOrderKey(CameraPosition).CompareTo(y.GetRenderOrderKey(CameraPosition));
        }
    }
}