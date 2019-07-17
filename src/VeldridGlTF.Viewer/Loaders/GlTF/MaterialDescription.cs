using System.Numerics;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class MaterialDescription : AbstractResource, IMaterialDescription
    {
        private bool _depthTestEnabled = true;
        private bool _depthWriteEnabled = true;

        public MaterialDescription(ResourceId id) : base(id)
        {
        }

        public IResourceHandler<ITexture> DiffuseTexture { get; set; }

        public Vector4 BaseColor { get; set; }

        public string ShaderName { get; set; } = "Default";

        public bool DepthTestEnabled
        {
            get { return _depthTestEnabled; }
            set { _depthTestEnabled = value; }
        }

        public bool DepthWriteEnabled
        {
            get { return _depthWriteEnabled; }
            set { _depthWriteEnabled = value; }
        }
    }
}