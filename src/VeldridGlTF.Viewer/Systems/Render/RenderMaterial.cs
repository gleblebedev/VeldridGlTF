using System.Numerics;
using System.Threading.Tasks;
using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderMaterial : IMaterial
    {
        private readonly ResourceId _id;

        public Vector4 DiffuseColor = Vector4.One;

        private IResourceHandler<ITexture> _diffuseTexture;
        private ResourceSet _materialSet;

        public RenderMaterial(ResourceId id)
        {
            _id = id;
        }

        public IResourceHandler<ITexture> DiffuseTexture
        {
            get => _diffuseTexture;
            set
            {
                if (_diffuseTexture != value)
                {
                    _diffuseTexture = value;
                    InvalidateMaterial();
                }
            }
        }

        private void InvalidateMaterial()
        {
            if (_materialSet != null)
            {
                _materialSet.Dispose();
                _materialSet = null;
            }
        }

        public ResourceSet MaterialSet
        {
            get { return _materialSet; }
        }

        public void EnsureResources(VeldridRenderSystem renderSystem)
        {
            if (_materialSet != null)
                return;
            if (_diffuseTexture != null && _diffuseTexture.Status != TaskStatus.RanToCompletion)
                return;
            _materialSet = renderSystem.CreateMaterialSet(this);
        }
    }
}