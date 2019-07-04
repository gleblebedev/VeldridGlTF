using System.Numerics;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class MaterialDescription : AbstractResource, IMaterialDescription
    {
        public MaterialDescription(ResourceId id) : base(id)
        {
        }

        public IResourceHandler<ITexture> DiffuseTexture { get; set; }

        public Vector4 BaseColor { get; set; }
    }
}