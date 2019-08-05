using System.Collections.Generic;
using System.Linq;
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

            Varyings.Add(WorldPosition = new VaryingDescription("v_WorldPosition", VaryingFormat.Float3));
            Varyings.Add(CameraPosition = new VaryingDescription("v_CameraPosition", VaryingFormat.Float3));

            var location = 0;
            foreach (var varying in Varyings)
            {
                varying.Location = location;
                location += varying.Size;
            }
        }

        public VaryingDescription WorldPosition { get; set; }
        public VaryingDescription CameraPosition { get; set; }
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


        public bool IsFlagSet(ShaderFlag flag)
        {
            return _shaderKey.IsFlagSet(flag);
        }
    }

    partial class FragmentShader : IShaderTemplate
    {
        public FragmentShader(DefaultShaderGenerator key)
        {
            Context = key;
        }

        public DefaultShaderGenerator Context { get; set; }
    }

    partial class VertexShader:IShaderTemplate
    {
        public VertexShader(DefaultShaderGenerator key)
        {
            Context = key;
        }

        public DefaultShaderGenerator Context { get; set; }
    }
}