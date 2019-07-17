using System;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class Zone : IZone, IDisposable
    {
        public VeldridRenderSystem RenderSystem { get; set; }

        public void Dispose()
        {
        }

        public IResourceHandler<IMaterial> Reflection { get; set; }
    }
}