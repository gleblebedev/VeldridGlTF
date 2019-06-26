using System.Numerics;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderMaterial : IMaterial
    {
        private readonly ResourceId _id;

        public Vector4 DiffuseColor = Vector4.One;

        public RenderMaterial(ResourceId id)
        {
            _id = id;
        }

        public IResourceHandler<ITexture> DiffuseTexture { get; set; }
    }
}