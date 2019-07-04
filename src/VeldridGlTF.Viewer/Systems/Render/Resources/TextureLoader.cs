using System.Threading.Tasks;
using Veldrid.ImageSharp;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class TextureLoader : IResourceLoader<ITexture>
    {
        private readonly VeldridRenderSystem _renderSystem;

        public TextureLoader(VeldridRenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public async Task<ITexture> LoadAsync(ResourceManager manager, ResourceId id)
        {
            var image = await manager.Resolve<IImage>(id).GetAsync();
            var graphicsDevice = await _renderSystem.GraphicsDevice;
            var resourceFactory = await _renderSystem.ResourceFactory;
            using (var stream = image.Open())
            {
                var deviceTexture = new ImageSharpTexture(stream).CreateDeviceTexture(graphicsDevice, resourceFactory);
                var view = resourceFactory.CreateTextureView(deviceTexture);
                return new TextureResource(id, deviceTexture, view);
            }
        }
    }
}