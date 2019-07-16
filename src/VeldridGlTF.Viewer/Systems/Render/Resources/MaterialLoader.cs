using System.Threading.Tasks;
using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MaterialLoader : IResourceLoader<IMaterial>
    {
        private readonly VeldridRenderSystem _renderSystem;

        public MaterialLoader(VeldridRenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public async Task<IMaterial> LoadAsync(ResourceContext context)
        {
            var description = await context.ResolveDependencyAsync<IMaterialDescription>(context.Id);
            var material = new MaterialResource(context.Id, _renderSystem);
            material._baseColor = description.BaseColor;
            material.DiffuseTexture = description.DiffuseTexture;
            var diffuse = await context.ResolveDependencyAsync(description.DiffuseTexture) as TextureResource;
            var renderContext = await context.ResolveDependencyAsync(_renderSystem.RenderContext);
            material.ResourceSet = renderContext.Factory.CreateResourceSet(new ResourceSetDescription(
                _renderSystem.MaterialLayout,
                diffuse?.View ?? _renderSystem.DefaultTextureView,
                renderContext.Device.Aniso4xSampler,
                _renderSystem.MaterialBuffer
            ));
            return material;
        }
    }
}