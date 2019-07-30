using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public abstract class AbstractRenderable
    {
        private IResourceHandler<DrawCallCollection> _cachedData;

        public VeldridRenderSystem RenderSystem { get; set; }


        public IResourceHandler<DrawCallCollection> GetDrawCalls()
        {
            return _cachedData ?? (_cachedData =
                       new ResourceHandler<DrawCallCollection>(ResourceId.Null, CreateRenderCache, null));
        }

        protected abstract Task<DrawCallCollection> CreateRenderCache(ResourceContext context);

        protected async Task<DrawCallCollection> CreateDrawCallCollection(Mesh mesh, MaterialResource material, RenderPass renderPass)
        {
            var drawCallCollection = new DrawCallCollection(mesh, mesh.Primitives.Count, await ResolveDrawCalls(mesh, material, renderPass));
            return drawCallCollection;
        }

        private async Task<DrawCall[]> ResolveDrawCalls(Mesh mesh, MaterialResource material, RenderPass renderPass)
        {
            var callTasks = new Task<DrawCall>[mesh.Primitives.Count];
            for (var index = 0; index < callTasks.Length; index++)
            {
                callTasks[index] = CreateDrawCall(mesh.Primitives[index], material, renderPass);
            }

            var calls = await Task.WhenAll(callTasks);
            return calls;
        }

        protected async Task<DrawCallCollection> CreateDrawCallCollection(Mesh mesh, IEnumerable<MaterialResource> materials, RenderPass renderPass)
        {
            var callTasks = new Task<DrawCall>[mesh.Primitives.Count];
            int index = 0;
            foreach (var material in materials)
            {
                if (index > callTasks.Length)
                    break;
                callTasks[index] = CreateDrawCall(mesh.Primitives[index], material, renderPass);
                ++index;
            }

            while (callTasks.Length > index)
            {
                callTasks[index] = Task.FromResult<DrawCall>(null);
                ++index;
            }
            var calls = await Task.WhenAll(callTasks);

            return new DrawCallCollection(mesh, mesh.Primitives.Count, calls);
        }

        protected async Task<DrawCall> CreateDrawCall(RenderPrimitive primitive, MaterialResource material, RenderPass renderPass)
        {
            if (material == null) return null;

            var PipelineBinder = await RenderSystem.GetPipeline(primitive, material, renderPass);
            var drawCall = new DrawCall
            {
                Pipeline = PipelineBinder,
                Material = material,
                Primitive = primitive
            };
            return drawCall;
        }
    }
}