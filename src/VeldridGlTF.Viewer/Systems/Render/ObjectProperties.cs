using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectProperties
    {
        public Matrix4x4 u_ModelMatrix;
        public Matrix4x4 u_NormalMatrix;
    }
}