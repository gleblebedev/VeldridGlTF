namespace VeldridGlTF.Viewer.Systems.Render
{
    public interface IShaderGenerator
    {
        string GetVertexShader();
        string GetFragmentShader();
    }
}