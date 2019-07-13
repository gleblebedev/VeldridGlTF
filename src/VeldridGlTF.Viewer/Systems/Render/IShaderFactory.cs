namespace VeldridGlTF.Viewer.Systems.Render
{
    public interface IShaderFactory
    {
        IShaderGenerator ResolveGenerator(ShaderKey key);
    }
}