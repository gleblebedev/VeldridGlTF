using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class StaticModel : AbstractRenderable, IStaticModel, IDisposable
    {
        private readonly DependencyProperty<MaterialSet> _materials = new DependencyProperty<MaterialSet>();

        private readonly DependencyProperty<IMesh> _mesh = new DependencyProperty<IMesh>();

        public void Dispose()
        {
            _mesh.Dispose();
            _materials.Dispose();
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
                if (_materials.TryGet(out var res)) return res;

                return null;
            }
            set => _materials.SetValue(value);
        }

        protected override async Task<DrawCallCollection> CreateRenderCache(ResourceContext context)
        {
            var mesh = await context.ResolveDependencyAsync(_mesh);
            var model = mesh as Mesh;
            var materialSet = await context.ResolveDependencyAsync(_materials);
            if (model == null)
                return null;
            if (materialSet == null)
                return null;
            var materials = new List<MaterialResource>(materialSet.Count);
            foreach (var material in materialSet)
                materials.Add(await context.ResolveDependencyAsync(material) as MaterialResource);
            return await CreateDrawCallCollection(model, materials, RenderSystem.MainPass);
        }
    }
}