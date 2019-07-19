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
            return await CreateMaterial(_renderSystem, context, description);
        }

        public static async Task<IMaterial> CreateMaterial(VeldridRenderSystem renderSystem, ResourceContext context,
            IMaterialDescription description)
        {
            var material = new MaterialResource(context.Id, renderSystem);
            material.ShaderName = description.ShaderName;
            material._baseColor = description.BaseColor;
            material.DiffuseTexture = description.DiffuseTexture;
            material.DepthStencilState.DepthTestEnabled = description.DepthTestEnabled;
            material.DepthStencilState.DepthWriteEnabled = description.DepthWriteEnabled;
            var diffuse = await context.ResolveDependencyAsync(description.DiffuseTexture) as TextureResource;
            var renderContext = await context.ResolveDependencyAsync(renderSystem.RenderContext);
            material.ResourceSet = renderContext.Factory.CreateResourceSet(new ResourceSetDescription(
                renderSystem.MaterialLayout,
                renderSystem.MaterialBuffer,
                diffuse?.View ?? renderSystem.DefaultTextureView,
                renderContext.Device.Aniso4xSampler
            ));
            return material;
        }
    }
}