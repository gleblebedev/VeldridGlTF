using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public interface ISkybox
    {
        IResourceHandler<IMaterial> Material { get; set; }
    }
}