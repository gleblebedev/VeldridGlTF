using System.Threading.Tasks;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MeshLoader : IResourceLoader<IMesh>
    {
        private readonly VeldridRenderSystem _renderSystem;

        public MeshLoader(VeldridRenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public async Task<IMesh> LoadAsync(ResourceContext context)
        {
            var geometry = await context.ResolveDependencyAsync<IGeometry>(context.Id);

            return await CreateMesh(context, geometry);
        }

        public Task<IMesh> CreateMesh(ResourceContext context, IGeometry geometry)
        {
            return Mesh.Create(_renderSystem, context, geometry);
        }
    }
}