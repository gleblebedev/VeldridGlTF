using VeldridGlTF.Viewer.Systems.Render.Resources;
using VeldridGlTF.Viewer.Systems.Render.Shaders;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public interface IShaderFactory
    {
        IShaderGenerator ResolveGenerator(ShaderKey key);
        ShaderKey GetShaderKey(RenderPrimitive primitive, MaterialResource material);
    }
}