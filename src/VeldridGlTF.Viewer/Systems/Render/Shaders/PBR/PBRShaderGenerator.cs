using System;
using System.Collections.Generic;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.PBR
{
    public class PBRShaderGenerator : IShaderGenerator
    {
        private readonly PBRShaderKey _shaderKey;

        public PBRShaderGenerator(PBRShaderKey shaderKey)
        {
            _shaderKey = shaderKey;
            Varyings.Add(new VaryingDescription("v_Position", VaryingFormat.Float3));
            if (HasFlag(ShaderFlag.HAS_NORMALS))
            {
                if (HasFlag(ShaderFlag.HAS_TANGENTS))
                    Varyings.Add(new VaryingDescription("v_TBN", VaryingFormat.Mat3));
                else
                    Varyings.Add(new VaryingDescription("v_Normal", VaryingFormat.Float3));
            }

            Varyings.Add(new VaryingDescription("v_UVCoord1", VaryingFormat.Float2));
            Varyings.Add(new VaryingDescription("v_UVCoord2", VaryingFormat.Float2));
            if (HasFlag(ShaderFlag.HAS_VERTEX_COLOR_VEC3))
                Varyings.Add(new VaryingDescription("v_Color", VaryingFormat.Float3));
            if (HasFlag(ShaderFlag.HAS_VERTEX_COLOR_VEC4))
                Varyings.Add(new VaryingDescription("v_Color", VaryingFormat.Float4));

            var location = 0;
            foreach (var varying in Varyings)
            {
                varying.Location = location;
                location += varying.Size;
            }
        }

        public IList<VaryingDescription> Varyings { get; } = new List<VaryingDescription>();

        public IList<VertexElementDescription> VertexElements =>
            _shaderKey.VertexLayout.VertexLayoutDescription.Elements;

        public uint JointCount => _shaderKey.JointCount;

        public string GetVertexShader()
        {
            return new VertexShader(this).TransformText();
        }

        public string GetFragmentShader()
        {
            return new FragmentShader(this).TransformText();
        }

        public bool HasFlag(ShaderFlag flag)
        {
            return _shaderKey.HasFlag(flag);
        }
    }

    partial class FragmentShader : IShaderTemplate
    {
        public FragmentShader(PBRShaderGenerator key)
        {
            Context = key;
        }

        public PBRShaderGenerator Context { get; set; }

        public void WriteMembers<T>()
        {
            Glsl.WriteMembers<T>(this);
        }

        public void WriteDefines()
        {
            foreach (ShaderFlag value in Enum.GetValues(typeof(ShaderFlag)))
                if (Context.HasFlag(value))
                    WriteLine("#define " + value);
        }
    }

    partial class VertexShader : IShaderTemplate
    {
        public VertexShader(PBRShaderGenerator key)
        {
            Context = key;
        }

        public PBRShaderGenerator Context { get; set; }

        public void WriteMembers<T>()
        {
            Glsl.WriteMembers<T>(this);
        }

        public void WriteDefines()
        {
            foreach (ShaderFlag value in Enum.GetValues(typeof(ShaderFlag)))
                if (Context.HasFlag(value))
                    WriteLine("#define " + value);
        }
    }
}