namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ShaderGenerator : IShaderGenerator
    {
        public string GetVertexShader(ShaderKey key)
        {
            return new VertexShader(key).TransformText();
        }

        public string GetFragmentShader(ShaderKey key)
        {
            return new FragmentShader(key).TransformText();
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
}