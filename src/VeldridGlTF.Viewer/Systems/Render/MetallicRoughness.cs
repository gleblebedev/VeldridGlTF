using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MetallicRoughness
    {
        [FieldOffset(0)] public Matrix3x3 BaseColorUVTransform;

        [FieldOffset(48)] public int BaseColorUVSet;

        [FieldOffset(64)] public Matrix3x3 MetallicRoughnessUVTransform;

        [FieldOffset(112)] public int MetallicRoughnessUVSet;

        [FieldOffset(116)] public float MetallicFactor;

        [FieldOffset(120)] public float RoughnessFactor;

        [FieldOffset(124)] public float AlphaCutoff;

        [FieldOffset(128)] public Vector4 BaseColorFactor;

        public static readonly MetallicRoughness Identity = new MetallicRoughness
        {
            BaseColorUVTransform = Matrix3x3.Identity,
            BaseColorUVSet = 0,
            MetallicRoughnessUVTransform = Matrix3x3.Identity,
            MetallicRoughnessUVSet = 0,
            MetallicFactor = 1.0f,
            RoughnessFactor = 1.0f,
            AlphaCutoff = 1.0f,
            BaseColorFactor = Vector4.One
        };
    }
}