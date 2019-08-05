using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Systems.Render.Resources;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Default;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.PBR
{
    public class PBRShaderFactory : IShaderFactory
    {
        public IShaderGenerator ResolveGenerator(ShaderKey key)
        {
            return new PBRShaderGenerator((PBRShaderKey)key);
        }

        public ShaderKey GetShaderKey(RenderPrimitive primitive, MaterialResource material, ILayoutNameResolver renderPass)
        {
            var shaderKey = new PBRShaderKey(this, renderPass, primitive.Elements);
            //if (material.DiffuseTexture != null)
            //{
            //    shaderKey.SetFlag(ShaderFlag.HAS_DIFFUSE_MAP);
            //}

            return shaderKey;
        }
    }
}
