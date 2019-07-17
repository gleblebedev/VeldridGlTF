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

        protected DrawCallCollection CreateDrawCallCollection(Mesh mesh, MaterialResource material)
        {
            return new DrawCallCollection(mesh, mesh.Primitives.Count,
                Enumerable.Range(0, mesh.Primitives.Count).Select(_ => CreateDrawCall(mesh.Primitives[_], material)));
        }

        protected DrawCallCollection CreateDrawCallCollection(Mesh mesh, IEnumerable<MaterialResource> materials)
        {
            return new DrawCallCollection(mesh, mesh.Primitives.Count,
                materials.Take(mesh.Primitives.Count).Select((_, i) => CreateDrawCall(mesh.Primitives[i], _)));
        }

        protected DrawCall CreateDrawCall(RenderPrimitive primitive, MaterialResource material)
        {
            if (material == null) return null;

            var pipeline = RenderSystem.GetPipeline(primitive, material);
            var drawCall = new DrawCall
            {
                Pipeline = pipeline,
                Material = material,
                Primitive = primitive
            };
            return drawCall;
        }
    }
}