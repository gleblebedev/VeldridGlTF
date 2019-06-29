namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ShaderGenerator: IShaderGenerator
    {

        public ShaderGenerator()
        {
        }

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
        public ShaderKey ShaderKey { get; set; }

        public FragmentShader(ShaderKey key)
        {
            ShaderKey = key;
        }
    }
    partial class VertexShader
    {
        public ShaderKey ShaderKey { get; set; }

        public VertexShader(ShaderKey key)
        {
            ShaderKey = key;
        }
    }
}