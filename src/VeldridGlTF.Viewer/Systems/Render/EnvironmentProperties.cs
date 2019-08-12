using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EnvironmentProperties
    {
        [FieldOffset(0)]
        public Matrix4x4 u_ViewProjectionMatrix;

        [FieldOffset(64)]
        public Vector3 u_Camera;

        [FieldOffset(76)]
        public float u_Exposure;

        [FieldOffset(80)]
        public int u_MipCount;
    }

}