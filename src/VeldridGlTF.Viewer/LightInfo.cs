using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LightInfo
    {
        public Vector3 Direction;
        private float _padding;
    }
}
