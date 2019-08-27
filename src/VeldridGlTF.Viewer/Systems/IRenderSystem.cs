using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;

namespace VeldridGlTF.Viewer.Systems
{
    public interface IRenderSystem
    {
        IStaticModel AddStaticModel(EcsEntity entity);

        ISkybox AddSkybox(EcsEntity entity);

        IZone AddZone(EcsEntity entity);
    }
}