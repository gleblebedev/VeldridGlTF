using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;

namespace VeldridGlTF.Viewer.Systems
{
    public interface IRenderSystem
    {
        IStaticModel AddStaticModel(EcsEntity entity);
    }
}
