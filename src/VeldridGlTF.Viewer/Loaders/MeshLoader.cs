using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Loaders
{
    public class MeshLoader : IResourceLoader<IMesh>
    {
        public async Task<IMesh> LoadAsync(ResourceManager manager, ResourceId id)
        {
            var context = await manager.Resolve<GlTFContainer>(new ResourceId(id.Container, null)).GetAsync();
            return await context.Meshes[id.Id].GetAsync();
        }
    }
}