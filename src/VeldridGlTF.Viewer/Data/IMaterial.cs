using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public interface IMaterial
    {
        IResourceHandler<ITexture> DiffuseTexture { get; set; }
    }
}