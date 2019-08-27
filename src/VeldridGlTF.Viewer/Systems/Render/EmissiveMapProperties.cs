using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EmissiveMapProperties
    {
        [FieldOffset(0)] public Vector3 EmissiveFactor;

        [FieldOffset(12)] public int EmissiveUVSet;

        [FieldOffset(16)] public Matrix3x3 EmissiveUVTransform;

        public static readonly EmissiveMapProperties Identity = new EmissiveMapProperties
        {
            EmissiveUVTransform = Matrix3x3.Identity,
            EmissiveFactor = Vector3.One,
            EmissiveUVSet = 0
        };
    }
}