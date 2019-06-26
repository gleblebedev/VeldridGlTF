using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;

namespace VeldridGlTF.Viewer.Systems
{
    [EcsInject]
    public class LocalToWorldSystem : IEcsRunSystem
    {
        private readonly EcsFilter<LocalTransform, WorldTransform> _filter = null;
        private EcsWorld _world = null;

        public void Run()
        {
            foreach (var index in _filter)
            {
                var local = _filter.Components1[index];
                var world = _filter.Components2[index];
                local.EvaluateWorldTransform(out world.WorldMatrix);
            }
        }
    }
}