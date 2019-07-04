using System.Threading.Tasks;
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

        public async Task<IMaterial> LoadAsync(ResourceManager manager, ResourceId id)
        {
            var description = await manager.Resolve<IMaterialDescription>(id).GetAsync();
            var material = new MaterialResource(id, _renderSystem);
            await material.UpdateAsync(description);
            return material;
        }
    }
}