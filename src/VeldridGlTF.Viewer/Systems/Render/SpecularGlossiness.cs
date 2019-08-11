using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SpecularGlossiness
    {
        [FieldOffset(0)]
        public Matrix3x3 DiffuseUVTransform;

        [FieldOffset(48)]
        public int DiffuseUVSet;

        [FieldOffset(64)]
        public Matrix3x3 SpecularGlossinessUVTransform;

        [FieldOffset(112)]
        public int SpecularGlossinessUVSet;

        [FieldOffset(128)]
        public Vector3 SpecularFactor;

        [FieldOffset(140)]
        public float GlossinessFactor;

        [FieldOffset(144)]
        public Vector4 DiffuseFactor;

        [FieldOffset(148)]
        public float AlphaCutoff;

        public static readonly SpecularGlossiness Identity = new SpecularGlossiness()
        {
            DiffuseUVTransform = Matrix3x3.Identity,
            DiffuseUVSet = 0,
            SpecularGlossinessUVTransform = Matrix3x3.Identity,
            SpecularGlossinessUVSet = 0,
            SpecularFactor = Vector3.One,
            GlossinessFactor = 1.0f,
            DiffuseFactor = Vector4.One,
            AlphaCutoff = 1.0f,
        };
    }
}