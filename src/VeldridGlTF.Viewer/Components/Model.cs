using System.Collections.Generic;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public class Model
    {
        private readonly IList<IResourceHandler<IMaterial>> _materials;

        public Model()
        {
            _materials = new MaterialCollection(this);
        }

        public void InvalidateRenderContext()
        {
            if (RenderContext != null)
            {
                RenderContext.Invalidate();
            }
        }


        public IResourceHandler<IMesh> Mesh { get; set; }

        public IList<IResourceHandler<IMaterial>> Materials => _materials;

        public IRenderContext RenderContext { get; set; }
    }
}