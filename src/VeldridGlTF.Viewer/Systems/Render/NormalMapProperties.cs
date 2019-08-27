using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Explicit)]
    public struct NormalMapProperties
    {
        [FieldOffset(0)] public Matrix3x3 NormalUVTransform;

        [FieldOffset(48)] public float NormalScale;

        [FieldOffset(52)] public int NormalUVSet;

        public static readonly NormalMapProperties Identity = new NormalMapProperties
        {
            NormalUVTransform = Matrix3x3.Identity,
            NormalScale = 1,
            NormalUVSet = 0
        };
    }
}