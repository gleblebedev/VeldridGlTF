using VeldridGlTF.Viewer.Data;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class DrawCall
    {
        public AlphaMode AlphaMode;
        public PipelineBinder Pipeline;
        public RenderPrimitive Primitive;
    }
}