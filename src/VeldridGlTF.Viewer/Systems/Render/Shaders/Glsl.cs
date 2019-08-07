using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.Shaders.PBR;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders
{
    public static class Glsl
    {
        public static string NameOf(VaryingFormat format)
        {
            switch (format)
            {
                case VaryingFormat.Float1:
                    return "float";
                case VaryingFormat.Float2:
                    return "vec2";
                case VaryingFormat.Float3:
                    return "vec3";
                case VaryingFormat.Float4:
                    return "vec4";
                case VaryingFormat.Mat3:
                    return "mat3";
                case VaryingFormat.Mat4:
                    return "mat4";
            }

            throw new NotImplementedException(format.ToString());
        }

        public static string NameOf(VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Float1:
                    return "float";
                case VertexElementFormat.Float2:
                    return "vec2";
                case VertexElementFormat.Float3:
                    return "vec3";
                case VertexElementFormat.Float4:
                    return "vec4";
                case VertexElementFormat.Byte2_Norm:
                    break;
                case VertexElementFormat.Byte2:
                    break;
                case VertexElementFormat.Byte4_Norm:
                    break;
                case VertexElementFormat.Byte4:
                    break;
                case VertexElementFormat.SByte2_Norm:
                    break;
                case VertexElementFormat.SByte2:
                    break;
                case VertexElementFormat.SByte4_Norm:
                    break;
                case VertexElementFormat.SByte4:
                    break;
                case VertexElementFormat.UShort2_Norm:
                    break;
                case VertexElementFormat.UShort2:
                    break;
                case VertexElementFormat.UShort4_Norm:
                    break;
                case VertexElementFormat.UShort4:
                    break;
                case VertexElementFormat.Short2_Norm:
                    break;
                case VertexElementFormat.Short2:
                    break;
                case VertexElementFormat.Short4_Norm:
                    break;
                case VertexElementFormat.Short4:
                    break;
                case VertexElementFormat.UInt1:
                    break;
                case VertexElementFormat.UInt2:
                    break;
                case VertexElementFormat.UInt3:
                    break;
                case VertexElementFormat.UInt4:
                    break;
                case VertexElementFormat.Int1:
                    break;
                case VertexElementFormat.Int2:
                    break;
                case VertexElementFormat.Int3:
                    break;
                case VertexElementFormat.Int4:
                    break;
                case VertexElementFormat.Half1:
                    break;
                case VertexElementFormat.Half2:
                    break;
                case VertexElementFormat.Half4:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            throw new NotImplementedException(format.ToString());
        }

        public static void WriteMembers<T>(IShaderTemplate template)
        {
            foreach (var fieldInfo in typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var fixedBuffer = (FixedBufferAttribute)fieldInfo.GetCustomAttribute(typeof(FixedBufferAttribute), false);
                if (fixedBuffer != null)
                {
                    template.WriteLine("    " + NameOf(fixedBuffer.ElementType) + " " + fieldInfo.Name + " ["+fixedBuffer.Length+"];");
                }
                else
                {
                    template.WriteLine("    " + NameOf(fieldInfo.FieldType) + " " + fieldInfo.Name + ";");
                }
            }
        }

        private static object NameOf(Type type)
        {
            if (type == typeof(float))
                return "float";
            if (type == typeof(int))
                return "int";
            if (type == typeof(Vector2))
                return "vec2";
            if (type == typeof(Vector3))
                return "vec3";
            if (type == typeof(Vector4))
                return "vec4";
            if (type == typeof(Matrix4x4))
                return "mat4";
            if (type == typeof(Matrix3x3))
                return "mat3";
            throw new NotImplementedException(type.FullName);
        }
    }
}