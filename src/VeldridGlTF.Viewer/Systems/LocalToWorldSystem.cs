using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;

namespace VeldridGlTF.Viewer.Systems
{
    [EcsInject]
    public class LocalToWorldSystem : IEcsRunSystem
    {
        EcsWorld _world = null;
        EcsFilter<LocalTransform, WorldTransform> _filter = null;

        public void Run()
        {
            foreach (var index in _filter)
            {
                var local = _filter.Components1[index];
                var world = _filter.Components2[index];
                local.EvaluateWorldTransform(out world.Transform);
                world.Transform.EvaluateMatrix(out world.WorldMatrix);
            }
        }
    }
}