using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems
{
    public interface IRenderSystem
    {
        IStaticModel AddStaticModel(EcsEntity entity);
    }
}
