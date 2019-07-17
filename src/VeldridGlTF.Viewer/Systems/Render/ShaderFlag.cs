using System;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [Flags]
    public enum ShaderFlag
    {
        None = 0,
        HAS_DIFFUSE_MAP = 1 << 0,
        HAS_SPECULAR_GLOSSINESS_MAP = 1 << 1,
        HAS_METALLIC_ROUGHNESS_MAP = 1 << 2,
        HAS_BASE_COLOR_MAP = 1 << 3,
        HAS_OCCLUSION_MAP = 1 << 4,
        HAS_EMISSIVE_MAP = 1 << 5,
        HAS_NORMALS = 1 << 6,
        HAS_TANGENTS = 1 << 7,
        HAS_UV_SET1 = 1 << 8,
        HAS_UV_SET2 = 1 << 9,
        HAS_VERTEX_COLOR = 1 << 10,
        USE_MORPHING = 1 << 11,
        USE_SKINNING = 1 << 12,
        MATERIAL_SPECULARGLOSSINESS = 1 << 13,
        MATERIAL_METALLICROUGHNESS = 1 << 14,
        ALPHAMODE_MASK = 1 << 15,
        MATERIAL_UNLIT = 1 << 16
    }
}