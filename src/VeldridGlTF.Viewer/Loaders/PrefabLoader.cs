using System.Linq;
using System.Threading.Tasks;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders
{
    public class PrefabLoader : IResourceLoader<EntityPrefab>
    {
       
        public async Task<EntityPrefab> LoadAsync(ResourceManager manager, ResourceId id)
        {
            var context = await manager.Resolve<GlTFContainer>(new ResourceId(id.Container, null)).GetAsync();
            if (id.Id == null)
            {
                return context.Root;
            }
            return context.GetEntity(id.Id);
        }

       
    }
}