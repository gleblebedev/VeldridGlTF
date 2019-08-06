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
            //SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.DiffuseSampler, ShaderFlag.HAS_DIFFUSE_MAP);
            //SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.MetallicRoughness, ShaderFlag.MATERIAL_METALLICROUGHNESS);
            //SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.BaseColorSampler, ShaderFlag.HAS_BASE_COLOR_MAP);
            //SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.SpecularGlossiness, ShaderFlag.MATERIAL_SPECULARGLOSSINESS);
            //SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.NormalSampler, ShaderFlag.HAS_NORMAL_MAP);
            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.EmissiveSampler, ShaderFlag.HAS_EMISSIVE_MAP);
            //SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.OcclusionSampler, ShaderFlag.HAS_OCCLUSION_MAP);
            if (material.Unlit)
                shaderKey.SetFlag(ShaderFlag.MATERIAL_UNLIT);
            return shaderKey;
        }

        private static void SetFlagIfPresent(MaterialResource material, PBRShaderKey shaderKey, string name, ShaderFlag flag)
        {
            if (material.ResourceSetBuilder.TryResolve(name, out var slot))
            {
                shaderKey.SetFlag(flag);
            }
        }
    }
}
