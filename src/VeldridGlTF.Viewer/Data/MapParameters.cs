using System.Numerics;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public class MapParameters
    {
        public IResourceHandler<ITexture> Map;
        public float Scale;
        public int UVSet = 0;
        public Matrix4x4 UVTransform = Matrix4x4.Identity;
    }
}