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

        public async Task<ITexture> LoadAsync(ResourceContext context)
        {
            var image = await context.ResolveDependencyAsync<IImage>(context.Id);
            var graphicsDevice = await context.ResolveDependencyAsync(_renderSystem.GraphicsDevice);
            var resourceFactory = await context.ResolveDependencyAsync(_renderSystem.ResourceFactory);
            using (var stream = image.Open())
            {
                var deviceTexture = new ImageSharpTexture(stream).CreateDeviceTexture(graphicsDevice, resourceFactory);
                var view = resourceFactory.CreateTextureView(deviceTexture);
                return new TextureResource(context.Id, deviceTexture, view);
            }
        }
    }
}