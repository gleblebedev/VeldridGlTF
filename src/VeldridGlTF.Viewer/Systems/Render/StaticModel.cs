using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veldrid;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class StaticModel : IStaticModel, IDisposable
    {

        private DependencyProperty<IMesh> _mesh = new DependencyProperty<IMesh>();
        private DependencyProperty<MaterialSet> _materials = new DependencyProperty<MaterialSet>();
        private IResourceHandler<ModelRenderCache> _cachedData;

        public StaticModel()
        {
        }

        public IResourceHandler<IMesh> Mesh
        {
            get => _mesh;
            set => _mesh.SetValue(value);
        }

        public MaterialSet Materials
        {
            get
            {
                if (_materials.TryGet(out var res))
                {
                    return res;
                }

                return null;
            }
            set => _materials.SetValue(value);
        }

        public VeldridRenderSystem RenderSystem { get; set; }

        public IResourceHandler<ModelRenderCache> GetRenderCache()
        {
            return _cachedData ?? (_cachedData = new ResourceHandler<ModelRenderCache>(ResourceId.Null, CreateRenderCache, null)); }

        private async Task<ModelRenderCache> CreateRenderCache(ResourceContext context)
        {
            var device = await context.ResolveDependencyAsync(RenderSystem.GraphicsDevice);
            var mesh = await context.ResolveDependencyAsync(_mesh);
            var model = mesh as RenderMesh;
            var materialSet = await context.ResolveDependencyAsync(_materials);
            if (model == null)
                return null;
            if (materialSet == null)
                return null;
            var materials = new List<MaterialResource>(materialSet.Count);
            foreach (var material in materialSet)
            {
                materials.Add(await context.ResolveDependencyAsync(material) as MaterialResource);
            }
            var cache = new ModelRenderCache();
            var numDrawCalls = Math.Min(materialSet.Count, model.Primitives.Count);
            cache.DrawCalls = new List<DrawCall>(numDrawCalls);
            cache.IndexBuffer = model._indexBuffer;
            cache.VertexBuffer = model._vertexBuffer;
            for (var index = 0; index < numDrawCalls; index++)
            {
                var indexRange = model.Primitives[index];
                var material = materials[index];
                if (material != null)
                {
                    var shaderKey = new ShaderKey {VertexLayout = indexRange.Elements};
                    if (material.DiffuseTexture != null &&
                        shaderKey.VertexLayout.VertexLayoutDescription.Elements.Any(_ => _.Name == "TEXCOORD_0"))
                        shaderKey.Flags |= ShaderFlag.DiffuseMap;
                    var pipeline = RenderSystem.GetPipeline(new PipelineKey
                    {
                        Shader = shaderKey,
                        PrimitiveTopology = indexRange.PrimitiveTopology
                    });
                    var drawCall = new DrawCall
                    {
                        Pipeline = pipeline,
                        Material = material,
                        Primitive = indexRange
                    };

                    cache.DrawCalls.Add(drawCall);
                }
            }

            return cache;
        }

        public void Dispose()
        {
            _mesh.Dispose();
            _materials.Dispose();
        }
    }
}