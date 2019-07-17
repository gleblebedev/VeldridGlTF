using System;
using System.Collections.Generic;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Default
{
    public class DefaultShaderGenerator : IShaderGenerator
    {
        private readonly DefaultShaderKey _shaderKey;

        public DefaultShaderGenerator(DefaultShaderKey shaderKey)
        {
            _shaderKey = shaderKey;
            if (_shaderKey.IsFlagSet(ShaderFlag.HAS_NORMALS))
            {
                if (_shaderKey.IsFlagSet(ShaderFlag.HAS_TANGENTS))
                    Varyings.Add(TBN = new VaryingDescription("v_TBN", VaryingFormat.Mat3));
                else
                    Varyings.Add(Normal = new VaryingDescription("v_NORMAL", VaryingFormat.Float3));
            }

            if (_shaderKey.IsFlagSet(ShaderFlag.HAS_UV_SET1))
                Varyings.Add(TexCoord0 = new VaryingDescription("v_TEXCOORD_0", VaryingFormat.Float2));
            if (_shaderKey.IsFlagSet(ShaderFlag.HAS_UV_SET2))
                Varyings.Add(TexCoord1 = new VaryingDescription("v_TEXCOORD_1", VaryingFormat.Float2));

            var location = 0;
            foreach (var varying in Varyings)
            {
                varying.Location = location;
                location += GetLocationSize(varying);
            }
        }

        public VaryingDescription Normal { get; set; }
        public VaryingDescription TBN { get; set; }

        public VaryingDescription TexCoord0 { get; set; }

        public VaryingDescription TexCoord1 { get; set; }


        public IList<VaryingDescription> Varyings { get; } = new List<VaryingDescription>();

        public IList<VertexElementDescription> VertexElements =>
            _shaderKey.VertexLayout.VertexLayoutDescription.Elements;

        public string GetVertexShader()
        {
            return new VertexShader(this).TransformText();
        }

        public string GetFragmentShader()
        {
            return new FragmentShader(this).TransformText();
        }

        private int GetLocationSize(VaryingDescription varying)
        {
            switch (varying.Format)
            {
                case VaryingFormat.Mat3:
                    return 3;
                default:
                    return 1;
            }
        }

        public bool IsFlagSet(ShaderFlag flag)
        {
            return _shaderKey.IsFlagSet(flag);
        }
    }

    partial class FragmentShader
    {
        public FragmentShader(DefaultShaderGenerator key)
        {
            Context = key;
        }

        public DefaultShaderGenerator Context { get; set; }
    }

    partial class VertexShader
    {
        public VertexShader(DefaultShaderGenerator key)
        {
            Context = key;
        }

        public DefaultShaderGenerator Context { get; set; }
    }

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
    }
}