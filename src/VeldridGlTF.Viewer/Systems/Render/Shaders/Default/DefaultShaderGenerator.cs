using System;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Default
{
    public class DefaultShaderGenerator : IShaderGenerator
    {
        public string GetVertexShader(ShaderKey key)
        {
            return new VertexShader(key).TransformText();
        }

        public string GetFragmentShader(ShaderKey key)
        {
            return new FragmentShader(key).TransformText();
        }

        public AbstractShaderKey GetShaderKey(PipelineKey pipelineKey)
        {
            throw new NotImplementedException();
        }
    }

    partial class FragmentShader
    {
        public FragmentShader(ShaderKey key)
        {
            ShaderKey = key;
        }

        public ShaderKey ShaderKey { get; set; }
    }

    partial class VertexShader
    {
        public VertexShader(ShaderKey key)
        {
            ShaderKey = key;
        }

        public ShaderKey ShaderKey { get; set; }
    }

    public static class Glsl
    {
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