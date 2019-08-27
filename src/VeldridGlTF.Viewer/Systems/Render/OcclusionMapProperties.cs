using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Explicit)]
    public struct OcclusionMapProperties
    {
        [FieldOffset(0)] public Matrix3x3 OcclusionUVTransform;

        [FieldOffset(48)] public float OcclusionStrength;

        [FieldOffset(52)] public int OcclusionUVSet;

        public static readonly OcclusionMapProperties Identity = new OcclusionMapProperties
        {
            OcclusionUVTransform = Matrix3x3.Identity,
            OcclusionStrength = 1,
            OcclusionUVSet = 0
        };
    }
}