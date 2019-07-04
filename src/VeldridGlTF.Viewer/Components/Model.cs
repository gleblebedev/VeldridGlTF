using System.Collections.Generic;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public class Model
    {
        public Model()
        {
            Materials = new MaterialCollection(this);
        }


        public IResourceHandler<IMesh> Mesh { get; set; }

        public IList<IResourceHandler<IMaterial>> Materials { get; }

        public IRenderContext RenderContext { get; set; }

        public void InvalidateRenderContext()
        {
            if (RenderContext != null) RenderContext.Invalidate();
        }
    }
}