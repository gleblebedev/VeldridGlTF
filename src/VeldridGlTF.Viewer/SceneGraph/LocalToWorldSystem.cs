using Leopotam.Ecs;

namespace VeldridGlTF.Viewer.SceneGraph
{
    [EcsInject]
    public class LocalToWorldSystem : IEcsRunSystem
    {
        private readonly Scene _scene;
        private EcsWorld _world = null;

        public LocalToWorldSystem(Scene scene)
        {
            _scene = scene;
        }

        public void Run()
        {
            _scene.UpdateWorldTransforms();
        }
    }
}