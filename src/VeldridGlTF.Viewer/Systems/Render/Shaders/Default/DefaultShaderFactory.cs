using System.Linq;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Default
{
    public class DefaultShaderFactory : IShaderFactory
    {
        public IShaderGenerator ResolveGenerator(ShaderKey key)
        {
            return new DefaultShaderGenerator((DefaultShaderKey) key);
        }

        public ShaderKey GetShaderKey(RenderPrimitive primitive, MaterialResource material, RenderPass renderPass)
        {
            var shaderKey = new DefaultShaderKey(this, renderPass, primitive.Elements);
            if (shaderKey.VertexLayout.VertexLayoutDescription.Elements.Any(_ => _.Name == "NORMAL"))
                shaderKey.Flags |= ShaderFlag.HAS_DIFFUSE_MAP;

            if (material.DiffuseTexture != null &&
                shaderKey.VertexLayout.VertexLayoutDescription.Elements.Any(_ => _.Name == "TEXCOORD_0"))
                shaderKey.Flags |= ShaderFlag.HAS_DIFFUSE_MAP;

            return shaderKey;
        }
    }
}