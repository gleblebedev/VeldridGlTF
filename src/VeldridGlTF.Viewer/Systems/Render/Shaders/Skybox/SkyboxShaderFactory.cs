using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Skybox
{
    public class SkyboxShaderGenerator : IShaderGenerator
    {
        public string GetVertexShader()
        {
            return @"#version 450

layout(set = 0, binding = 0) uniform EnvironmentProperties
{
    mat4 u_ViewProjectionMatrix;
	vec3 u_Camera;
};


layout(location = 0) in vec3 POSITION;
layout(location = 0) out vec3 fsin_0;

layout(constant_id = 102) const bool ReverseDepthRange = true;

void main()
{
    vec4 pos = u_ViewProjectionMatrix * vec4(POSITION + u_Camera, 1.0f);
    gl_Position = vec4(pos.x, pos.y, pos.w, pos.w);
    if (ReverseDepthRange) { gl_Position.z = 0; }
    fsin_0 = POSITION;
}";
        }

        public string GetFragmentShader()
        {
            return @"#version 450

layout(set = 3, binding = 1) uniform textureCube SurfaceTexture;
layout(set = 3, binding = 2) uniform sampler SurfaceSampler;

layout(location = 0) in vec3 fsin_0;
layout(location = 0) out vec4 OutputColor;

void main()
{
	//vec4 reflection = texture(samplerCube(ReflectionTexture, ReflectionSampler), vec3(0,0,0));
	//vec4 brdf = texture(sampler2D(BRDFTexture, BRDFSampler), vec2(0.0, 0.0));
    vec4 skybox = texture(samplerCube(SurfaceTexture, SurfaceSampler), fsin_0);
    OutputColor = skybox;
}";
        }
    }

    public class SkyboxShaderFactory : IShaderFactory
    {
        public IShaderGenerator ResolveGenerator(ShaderKey key)
        {
            return new SkyboxShaderGenerator();
        }

        public ShaderKey GetShaderKey(RenderPrimitive primitive, MaterialResource material, ILayoutNameResolver pass)
        {
            return new ShaderKey(this, pass);
        }
    }
}