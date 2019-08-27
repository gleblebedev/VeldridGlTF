using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer.Systems.Render.Uniforms
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ObjectProperties
    {
        [FieldOffset(0)] public Matrix4x4 ModelMatrix;

        [FieldOffset(64)] public Matrix4x4 NormalMatrix;

        [FieldOffset(128)] public fixed float MorphWeights [5];
    }

    public static unsafe class ExtensionMethods
    {
        public static ref ObjectProperties AsObjectProperties(this byte[] buffer)
        {
            fixed (byte* ptr = buffer)
            {
                return ref *(ObjectProperties*) ptr;
            }
        }

        public static ref ObjectProperties AsObjectProperties(this Span<byte> buffer)
        {
            fixed (byte* ptr = buffer)
            {
                return ref *(ObjectProperties*) ptr;
            }
        }

        public static ref ObjectProperties AsObjectProperties(this ArraySegment<byte> buffer)
        {
            fixed (byte* ptr = buffer.Array)
            {
                return ref *(ObjectProperties*) (ptr + buffer.Offset);
            }
        }
    }
}