using System.Threading.Tasks;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MaterialLoader : ResourceLoader<IMaterial>
    {
        private readonly VeldridRenderSystem _renderSystem;

        public MaterialLoader(VeldridRenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public override async Task<IMaterial> LoadAsync(ResourceContext context)
        {
            var description = await context.ResolveDependencyAsync<IMaterialDescription>(context.Id);
            var material = new MaterialResource(context.Id, _renderSystem);
            await material.UpdateAsync(description);
            return material;
        }
    }
}