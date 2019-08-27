using Veldrid;
using VeldridGlTF.Viewer.Data;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class PipelineAndLayouts
    {
        public AlphaMode AlphaMode;
        public ResourceLayoutDescription[] Layouts;
        public Pipeline Pipeline;
        public ResourceLayout[] ResourceLayouts;
    }
}