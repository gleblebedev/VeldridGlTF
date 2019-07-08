using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Components
{
    public interface IStaticModel
    {
        IResourceHandler<IMesh> Mesh { get; set; }
        MaterialSet Materials { get; set; }
    }
}