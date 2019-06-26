using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public class StaticModel
    {
        public IResourceHandler<IMesh> Model { get; set; }

        public IResourceHandler<IMaterial> Material { get; set; }
    }
}