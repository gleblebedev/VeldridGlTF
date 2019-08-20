using System.Numerics;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public class MapParameters
    {
        public IResourceHandler<ITexture> Map;
        public float Scale;
        public int UVSet = 0;
        public Matrix3x3 UVTransform = Matrix3x3.Identity;
        public Vector4 Color;
        public WrapMode AddressModeU = WrapMode.Wrap;
        public WrapMode AddressModeV = WrapMode.Wrap;
        public WrapMode AddressModeW = WrapMode.Wrap;
    }
}