namespace VeldridGlTF.Viewer.Systems.Render
{
    public interface IShaderGenerator
    {
        string GetVertexShader(ShaderKey key);
        string GetFragmentShader(ShaderKey key);
    }
}