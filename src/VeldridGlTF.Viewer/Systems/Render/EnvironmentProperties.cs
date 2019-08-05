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
}