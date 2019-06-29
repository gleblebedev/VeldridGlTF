using System;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Loaders;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer
{
    public class SceneRenderer : IDisposable
    {
        private ResourceManager _resourceManager;
        private readonly StepContext _stepContext;

        private readonly VeldridRenderSystem _veldridRenderSystem;
        private VeldridGlTF.Viewer.SceneGraph.Scene _scene = new VeldridGlTF.Viewer.SceneGraph.Scene();

        public SceneRenderer(IApplicationWindow window)
        {
            Window = window;

            _stepContext = new StepContext();
            _veldridRenderSystem = new VeldridRenderSystem(_stepContext, window);
            _scene.Systems
                .Add(new LocalToWorldSystem(_scene))
                .Add(_veldridRenderSystem);
            _scene.Systems.Initialize();
            _resourceManager = new ResourceManager();

            var e = LoadGlTFSample().Result;


            //for (int x=-1; x<=1; ++x)
            //for (int y = -1; y <= 1; ++y)
            //{
            //    LocalTransform lt;
            //    WorldTransform wt;
            //    StaticModel sm;
            //    EcsEntity entity = _world.CreateEntityWith<LocalTransform, WorldTransform, StaticModel>(out lt, out wt, out sm);
            //    lt.Transform = new Transform(new Vector3(x,y,0), Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.One), x+y));
            //}

            Window.Rendering += PreDraw;
            Window.Rendering += Draw;
        }

        public IApplicationWindow Window { get; }

        public void Dispose()
        {
            _scene.Dispose();
        }

        private async Task<SceneGraph.Node> LoadGlTFSample()
        {
            _resourceManager = new ResourceManager()
                .With(new GlTFLoader())
                .With(new PrefabLoader())
                .With(new MeshLoader());
            //var container = await _resourceManager
              //  .Resolve<GlTFContainer>(new ResourceId("VeldridGlTF.Viewer.Assets.Buggy.glb", null)).GetAsync();
                //foreach (var mesh in container.Meshes) await mesh.GetAsync();
            var prefab = await _resourceManager
                .Resolve<EntityPrefab>(new ResourceId("VeldridGlTF.Viewer.Assets.Buggy.glb", null)).GetAsync();
            return prefab.Spawn(_scene);
        }

        private void PreDraw(float deltaSeconds)
        {
        }

        protected void Draw(float deltaSeconds)
        {
            _stepContext.DeltaSeconds = deltaSeconds;
            _scene.Systems.Run();
        }
    }
}