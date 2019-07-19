using System;
using System.Numerics;
using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MaterialResource : AbstractResource, IMaterial, IDisposable
    {
        private readonly VeldridRenderSystem _renderSystem;
        public Vector4 _baseColor;
        private ResourceSet _resourceSet;

        public DepthStencilStateDescription DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;


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

        public override void Dispose()
        {
            ResourceSet?.Dispose();
            base.Dispose();
        }

        public IResourceHandler<ITexture> DiffuseTexture { get; set; }

        public string ShaderName { get; set; } = "Default";

        public void UpdateBuffer(CommandList _cl, DeviceBuffer materialBuffer)
        {
            _cl.UpdateBuffer(materialBuffer, 0, ref _baseColor);
        }
    }
}