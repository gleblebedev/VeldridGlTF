using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class DrawCall
    {
        public MaterialResource Material;
        public Pipeline Pipeline;
        public RenderPrimitive Primitive;
    }
}