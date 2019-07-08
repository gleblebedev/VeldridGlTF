using System.Numerics;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public interface IMaterialDescription
    {
        IResourceHandler<ITexture> DiffuseTexture { get; set; }
        Vector4 BaseColor { get; set; }
    }
}