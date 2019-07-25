using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class DrawCall
    {
        public MaterialResource Material;
        public PipelineBinder Pipeline;
        public RenderPrimitive Primitive;
    }
}