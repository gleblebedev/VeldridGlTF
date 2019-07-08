using System;
using System.Numerics;
using System.Threading.Tasks;
using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MaterialResource : AbstractResource, IMaterial
    {
        private readonly VeldridRenderSystem _renderSystem;
        public Vector4 _baseColor;
        private ResourceSet _resourceSet;
        private IResourceHandler<ITexture> _diffuseTexture;


        public MaterialResource(ResourceId id, VeldridRenderSystem renderSystem) : base(id)
        {
            _renderSystem = renderSystem;
        }

        public ResourceSet ResourceSet
        {
            get => _resourceSet;
            internal set
            {
                if (_resourceSet != null) _resourceSet.Dispose();
                _resourceSet = value;
            }
        }

        public IResourceHandler<ITexture> DiffuseTexture 
        {
            get { return _diffuseTexture; }
            set { _diffuseTexture = value; }
        }

        public void UpdateBuffer(CommandList _cl, DeviceBuffer materialBuffer)
        {
            _cl.UpdateBuffer(materialBuffer, 0, ref _baseColor);
        }
    }
}