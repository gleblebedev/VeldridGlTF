using System.Numerics;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public interface IMaterialDescription
    {
        IResourceHandler<ITexture> DiffuseTexture { get; }
        Vector4 BaseColor { get; }
        string ShaderName { get; }
        bool DepthTestEnabled { get; }
        bool DepthWriteEnabled { get; }
    }
}