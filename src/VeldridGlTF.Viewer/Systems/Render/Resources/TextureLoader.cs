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
            var renderContext = await context.ResolveDependencyAsync(_renderSystem.RenderContext);
            using (var stream = image.Open())
            {
                var deviceTexture =
                    new ImageSharpTexture(stream).CreateDeviceTexture(renderContext.Device, renderContext.Factory);
                //deviceTexture.Name = context.Id.ToString();
                var view = renderContext.Factory.CreateTextureView(deviceTexture);
                //view.Name = deviceTexture.Name;
                return new TextureResource(context.Id, deviceTexture, view);
            }
        }
    }
}