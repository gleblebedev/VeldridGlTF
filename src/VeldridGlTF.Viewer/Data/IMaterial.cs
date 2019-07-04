using System.Numerics;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public interface IMaterialDescription
    {
        IResourceHandler<ITexture> DiffuseTexture { get; set; }
        Vector4 BaseColor { get; set; }
    }

    public interface IMaterial
    {
        IResourceHandler<ITexture> DiffuseTexture { get; }
        Task SetDiffuseTextureAsync(IResourceHandler<ITexture> texture);
    }
}