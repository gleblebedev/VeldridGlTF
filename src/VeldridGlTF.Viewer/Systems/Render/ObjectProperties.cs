using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ObjectProperties
    {
        [FieldOffset(0)]
        public Matrix4x4 ModelMatrix;
        [FieldOffset(64)]
        public Matrix4x4 NormalMatrix;
        [FieldOffset(128)]
        public fixed float MorphWeights[5];
    }
}