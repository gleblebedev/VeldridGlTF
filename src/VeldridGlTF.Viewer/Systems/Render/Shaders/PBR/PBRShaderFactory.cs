using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.PBR
{
    public class PBRShaderFactory : IShaderFactory
    {
        public IShaderGenerator ResolveGenerator(ShaderKey key)
        {
            return new PBRShaderGenerator((PBRShaderKey) key);
        }

        public ShaderKey GetShaderKey(RenderPrimitive primitive,  MaterialResource material,
            ILayoutNameResolver renderPass)
        {
            var shaderKey = new PBRShaderKey(this, renderPass, primitive.Elements, primitive.JointCount);
            //if (material.DiffuseTexture != null)
            //{
            //    shaderKey.SetFlag(ShaderFlag.HAS_DIFFUSE_MAP);
            //}

            if (primitive.JointCount > 0
                &&
                ((shaderKey.HasFlag(ShaderFlag.HAS_JOINT_SET1) && shaderKey.HasFlag(ShaderFlag.HAS_WEIGHT_SET1)) ||
                 (shaderKey.HasFlag(ShaderFlag.HAS_JOINT_SET2) && shaderKey.HasFlag(ShaderFlag.HAS_WEIGHT_SET2)))
            )
            {
                shaderKey.SetFlag(ShaderFlag.USE_SKINNING);
            }


            var hasSpecGloss =
                material.ResourceSetBuilder.TryResolve(MaterialResource.Slots.SpecularGlossiness, out var specGloss);
            var hasMetRough =
                material.ResourceSetBuilder.TryResolve(MaterialResource.Slots.MetallicRoughness, out var metRough);

            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.DiffuseSampler, ShaderFlag.HAS_DIFFUSE_MAP);
            if (shaderKey.HasFlag(ShaderFlag.HAS_DIFFUSE_MAP)) shaderKey.SetFlag(ShaderFlag.HAS_DIFFUSE_UV_TRANSFORM);
            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.SpecularGlossinessSampler,
                ShaderFlag.HAS_SPECULAR_GLOSSINESS_MAP);
            if (shaderKey.HasFlag(ShaderFlag.HAS_SPECULAR_GLOSSINESS_MAP))
                shaderKey.SetFlag(ShaderFlag.HAS_SPECULARGLOSSINESS_UV_TRANSFORM);
            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.SpecularGlossiness,
                ShaderFlag.MATERIAL_SPECULARGLOSSINESS);


            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.BaseColorSampler,
                ShaderFlag.HAS_BASE_COLOR_MAP);
            if (shaderKey.HasFlag(ShaderFlag.HAS_BASE_COLOR_MAP))
                shaderKey.SetFlag(ShaderFlag.HAS_BASECOLOR_UV_TRANSFORM);
            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.MetallicRoughnessSampler,
                ShaderFlag.HAS_METALLIC_ROUGHNESS_MAP);
            if (shaderKey.HasFlag(ShaderFlag.HAS_METALLIC_ROUGHNESS_MAP))
                shaderKey.SetFlag(ShaderFlag.HAS_METALLICROUGHNESS_UV_TRANSFORM);
            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.MetallicRoughness,
                ShaderFlag.MATERIAL_METALLICROUGHNESS);

            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.NormalSampler, ShaderFlag.HAS_NORMAL_MAP);
            if (shaderKey.HasFlag(ShaderFlag.HAS_NORMAL_MAP)) shaderKey.SetFlag(ShaderFlag.HAS_NORMAL_UV_TRANSFORM);
            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.EmissiveSampler, ShaderFlag.HAS_EMISSIVE_MAP);
            if (shaderKey.HasFlag(ShaderFlag.HAS_EMISSIVE_MAP)) shaderKey.SetFlag(ShaderFlag.HAS_EMISSIVE_UV_TRANSFORM);
            SetFlagIfPresent(material, shaderKey, MaterialResource.Slots.OcclusionSampler,
                ShaderFlag.HAS_OCCLUSION_MAP);
            if (shaderKey.HasFlag(ShaderFlag.HAS_OCCLUSION_MAP))
                shaderKey.SetFlag(ShaderFlag.HAS_OCCLSION_UV_TRANSFORM);

            if ((hasSpecGloss || hasMetRough) && material.AlphaMode == AlphaMode.Mask)
                shaderKey.SetFlag(ShaderFlag.ALPHAMODE_MASK);

            if (material.Unlit) shaderKey.SetFlag(ShaderFlag.MATERIAL_UNLIT);

            shaderKey.SetFlag(ShaderFlag.USE_IBL);
            shaderKey.SetFlag(ShaderFlag.USE_TEX_LOD);

            if (shaderKey.HasFlag(ShaderFlag.HAS_TARGET_POSITION0) ||
                shaderKey.HasFlag(ShaderFlag.HAS_TARGET_NORMAL0) ||
                shaderKey.HasFlag(ShaderFlag.HAS_TARGET_TANGENT0))
                shaderKey.SetFlag(ShaderFlag.USE_MORPHING);

            return shaderKey;
        }

        private static void SetFlagIfPresent(MaterialResource material, PBRShaderKey shaderKey, string name,
            ShaderFlag flag)
        {
            if (material.ResourceSetBuilder.TryResolve(name, out var slot)) shaderKey.SetFlag(flag);
        }
    }
}