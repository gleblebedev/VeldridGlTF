using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class DrawCall
    {
        public PipelineBinder Pipeline;
        public RenderPrimitive Primitive;
        public AlphaMode AlphaMode;
    }
}