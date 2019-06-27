using System.Collections.Generic;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public class StaticModel
    {
        public IResourceHandler<IMesh> Model { get; set; }

        public IList<IResourceHandler<IMaterial>> Materials { get; } = new List<IResourceHandler<IMaterial>>();
    }
}