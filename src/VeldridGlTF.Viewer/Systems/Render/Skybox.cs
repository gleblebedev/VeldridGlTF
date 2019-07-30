using System;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class Skybox : AbstractRenderable, ISkybox, IDisposable
    {
        private readonly DependencyProperty<IMaterial> _material = new DependencyProperty<IMaterial>();
        private IResourceHandler<IMesh> _mesh;

        public IResourceHandler<IMesh> Mesh => _mesh ?? (_mesh = CreateMesh());

        public void Dispose()
        {
        }

        public IResourceHandler<IMaterial> Material
        {
            get => _material;
            set => _material.SetValue(value);
        }

        private IResourceHandler<IMesh> CreateMesh()
        {
            return new ResourceHandler<IMesh>(ResourceId.Null,
                _ => Resources.Mesh.Create(RenderSystem, _, SkyboxGeometry.Handler), null);
        }

        protected override async Task<DrawCallCollection> CreateRenderCache(ResourceContext context)
        {
            var mesh = await context.ResolveDependencyAsync(Mesh) as Mesh;
            if (mesh == null)
                return null;
            var material = await context.ResolveDependencyAsync(_material) as MaterialResource;
            if (material == null)
                return null;
            return await CreateDrawCallCollection(mesh, material, RenderSystem.MainPass);
        }
    }
}