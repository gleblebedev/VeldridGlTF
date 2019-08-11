using Veldrid;
using VeldridGlTF.Viewer.Data;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class PipelineAndLayouts
    {
        public Pipeline Pipeline;
        public ResourceLayoutDescription[] Layouts;
        public ResourceLayout[] ResourceLayouts;
        public AlphaMode AlphaMode;
    }
}