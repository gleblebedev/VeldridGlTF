using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraInfo
    {
        public Vector3 CameraPosition_WorldSpace;
        private float _padding1;
        public Vector3 CameraLookDirection;
        private float _padding2;
    }
}