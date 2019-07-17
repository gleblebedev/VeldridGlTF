using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public interface IZone
    {
        IResourceHandler<IMaterial> Reflection { get; set; }
    }
}