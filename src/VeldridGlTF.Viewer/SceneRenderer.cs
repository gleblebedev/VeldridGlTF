using System;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Loaders.GlTF;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.SceneGraph;
using VeldridGlTF.Viewer.Systems.Render;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer
{
    public class SceneRenderer : IDisposable
    {
        private readonly ResourceManager _resourceManager;
        private readonly Scene _scene = new Scene();
        private readonly StepContext _stepContext;

        private readonly VeldridRenderSystem _veldridRenderSystem;

        private readonly object _gate = new object();

        public SceneRenderer(IApplicationWindow window)
        {
            Window = window;

            _stepContext = new StepContext();
            _veldridRenderSystem = new VeldridRenderSystem(_stepContext, window);
            _scene.Systems
                .Add(new LocalToWorldSystem(_scene))
                .Add(_veldridRenderSystem);
            _scene.Systems.Initialize();
            _resourceManager = new ResourceManager()
                    .With(new ContainerLoader())
                    .WithContainer<IGeometry>()
                    .WithContainer<IImage>()
                    .WithContainer<IMaterialDescription>()
                    .With(new PrefabLoader())
                    .With(new TextureLoader(_veldridRenderSystem))
                    .With(new MaterialLoader(_veldridRenderSystem))
                    .With(new MeshLoader(_veldridRenderSystem))
                ;

            Task.Run(LoadGlTFSample);

            Window.Rendering += PreDraw;
            Window.Rendering += Draw;
        }

        public IApplicationWindow Window { get; }

        public void Dispose()
        {
            _scene.Dispose();
        }


        private async Task<Node> LoadGlTFSample()
        {
            var resourceId = new ResourceId("VeldridGlTF.Viewer.Assets.Buggy.glb", null);
            //var resourceId = new ResourceId("VeldridGlTF.Viewer.Assets.BoomBox.glb", null);

            var prefab = await _resourceManager.Resolve<EntityPrefab>(resourceId).GetAsync();
            lock (_gate)
            {
                var node = prefab.Spawn(_scene);
                //node.Transform.Scale = Vector3.One * 10000;
                return node;
            }
        }

        private void PreDraw(float deltaSeconds)
        {
        }

        protected void Draw(float deltaSeconds)
        {
            _stepContext.DeltaSeconds = deltaSeconds;
            lock (_gate)
            {
                _scene.Systems.Run();
            }
        }
    }
}