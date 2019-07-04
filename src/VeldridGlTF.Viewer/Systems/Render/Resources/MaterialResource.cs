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
        private Vector4 _baseColor;
        private ResourceSet _resourceSet;


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

        public IResourceHandler<ITexture> DiffuseTexture { get; private set; }

        public Task SetDiffuseTextureAsync(IResourceHandler<ITexture> texture)
        {
            DiffuseTexture = texture;
            if (ResourceSet == null) return Task.CompletedTask;
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(IMaterialDescription description)
        {
            DiffuseTexture = description.DiffuseTexture;
            _baseColor = description.BaseColor;
            var diffuse = (TextureResource) await DiffuseTexture.GetAsyncOrDefault();
            var resourceFactory = await _renderSystem.ResourceFactory;
            var graphicsDevice = await _renderSystem.GraphicsDevice;
            ResourceSet = resourceFactory.CreateResourceSet(new ResourceSetDescription(
                _renderSystem.MaterialLayout,
                diffuse?.View ?? _renderSystem.DefaultTextureView,
                graphicsDevice.Aniso4xSampler,
                _renderSystem.MaterialBuffer
            ));
        }

        public void UpdateBuffer(CommandList _cl, DeviceBuffer materialBuffer)
        {
            _cl.UpdateBuffer(materialBuffer, 0, ref _baseColor);
        }
    }
}