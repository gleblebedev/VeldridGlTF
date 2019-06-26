using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public class StaticModel
    {
        public IResourceHandler<IMesh> Model
        {
            get;
            set;
        }
    }
}
