using System;
using System.Threading.Tasks;
using Leopotam.Ecs;
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
        private EcsSystems _systems;

        private readonly VeldridRenderSystem _veldridRenderSystem;
        private EcsWorld _world;

        public SceneRenderer(IApplicationWindow window)
        {
            Window = window;

            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _stepContext = new StepContext();
            _veldridRenderSystem = new VeldridRenderSystem(_stepContext, window);
            _systems
                .Add(new LocalToWorldSystem())
                .Add(_veldridRenderSystem);
            _systems.Initialize();
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
            _systems.Dispose();
            _systems = null;
            _world.Dispose();
            _world = null;
        }

        private async Task<EcsEntity> LoadGlTFSample()
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
            return prefab.Spawn(_world);
        }

        private void PreDraw(float deltaSeconds)
        {
        }

        protected void Draw(float deltaSeconds)
        {
            _stepContext.DeltaSeconds = deltaSeconds;
            _systems.Run();
        }
    }
}