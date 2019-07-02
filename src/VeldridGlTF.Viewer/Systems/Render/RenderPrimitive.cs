using System.Collections.Generic;
using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.ElementAccessors;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public struct RenderPrimitive
    {
        public uint DataOffset;
        public uint Start;
        public uint Length;
        public RenderVertexLayout Elements;
        public PrimitiveTopology PrimitiveTopology;
    }
}