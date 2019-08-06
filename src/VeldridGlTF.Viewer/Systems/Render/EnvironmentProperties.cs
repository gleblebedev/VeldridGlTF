using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EnvironmentProperties
    {
        public Matrix4x4 u_ViewProjectionMatrix;
        public Vector3 u_Camera;
        //private float padding0;
        public float u_Exposure;
        public int u_MipCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EmissiveMapProperties
    {
        public Vector3 u_EmissiveFactor;
        public int u_EmissiveUVSet;
        public Matrix3x3 u_EmissiveUVTransform;
        private float _padding0;
        private float _padding1;
        private float _padding2;

        public static readonly EmissiveMapProperties Identity = new EmissiveMapProperties()
        {
            u_EmissiveUVTransform = Matrix3x3.Identity,
            u_EmissiveFactor = Vector3.One,
            u_EmissiveUVSet = 0
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MetallicRoughness
    {
    }
}