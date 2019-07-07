using System;
using System.Numerics;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Loaders.FileSystem;
using VeldridGlTF.Viewer.Loaders.GlTF;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.SceneGraph;
using VeldridGlTF.Viewer.Systems.Render;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer
{
    public class SceneRenderer : IDisposable
    {
        private readonly object _gate = new object();
        private readonly ResourceManager _resourceManager;
        private readonly Scene _scene = new Scene();
        private readonly StepContext _stepContext;

        private readonly VeldridRenderSystem _veldridRenderSystem;

        public SceneRenderer(IApplicationWindow window, ViewerOptions options)
        {
            Window = window;

            _stepContext = new StepContext();
            _veldridRenderSystem = new VeldridRenderSystem(_stepContext, window);
            _scene.Systems
                .Add(new LocalToWorldSystem(_scene))
                .Add(_veldridRenderSystem);
            _scene.Systems.Initialize();
            FileCollection fileCollection;
            if (options.RootFolder != null)
                fileCollection = new FileCollection(options.RootFolder);
            else
                fileCollection = new FileCollection(options.DataFolder);
            _resourceManager = new ResourceManager()
                    .With(fileCollection)
                    .With(new ContainerLoader(), ".glb", ".gltf")
                    .With(new TextureLoader(_veldridRenderSystem))
                    .With(new MaterialLoader(_veldridRenderSystem))
                    .With(new MeshLoader(_veldridRenderSystem))
                ;

            Task.Run(() => LoadGlTFSample(options.FileName, options.Scale));

            Window.Rendering += PreDraw;
            Window.Rendering += Draw;
        }

        public IApplicationWindow Window { get; }

        public void Dispose()
        {
            _scene.Dispose();
        }


        private async Task<Node> LoadGlTFSample(string name, float scale)
        {
            var resourceId = new ResourceId(name, null);

            var prefab = await _resourceManager.Resolve<EntityPrefab>(resourceId).GetAsync();
            lock (_gate)
            {
                var node = prefab.Spawn(_scene);
                node.Transform.Scale = Vector3.One * scale;
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