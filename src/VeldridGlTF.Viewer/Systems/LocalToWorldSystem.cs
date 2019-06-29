using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.SceneGraph;

namespace VeldridGlTF.Viewer.Systems
{
    [EcsInject]
    public class LocalToWorldSystem : IEcsRunSystem
    {
        private readonly Scene _scene;

        public LocalToWorldSystem(Scene scene)
        {
            _scene = scene;
        }
        private EcsWorld _world = null;

        public void Run()
        {
            _scene.UpdateWorldTransforms();
        }
    }
}