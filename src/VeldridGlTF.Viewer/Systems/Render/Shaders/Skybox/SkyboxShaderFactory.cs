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
    mat4 View;
    mat4 Projection;
	vec3 CameraPos;
};


layout(location = 0) in vec3 POSITION;
layout(location = 0) out vec3 fsin_0;

layout(constant_id = 102) const bool ReverseDepthRange = true;

void main()
{
    mat4 view3x3 = mat4(
        View[0][0], View[0][1], View[0][2], 0,
        View[1][0], View[1][1], View[1][2], 0,
        View[2][0], View[2][1], View[2][2], 0,
        0, 0, 0, 1);
    vec4 pos = Projection * view3x3 * vec4(POSITION, 1.0f);
    gl_Position = vec4(pos.x, pos.y, pos.w, pos.w);
    if (ReverseDepthRange) { gl_Position.z = 0; }
    fsin_0 = POSITION;
}";
        }

        public string GetFragmentShader()
        {
            return @"#version 450

//layout(set = 0, binding = 1) uniform texture2D BRDFTexture;
//layout(set = 0, binding = 2) uniform sampler BRDFSampler;

//layout(set = 1, binding = 0) uniform textureCube ReflectionTexture;
//layout(set = 1, binding = 1) uniform sampler ReflectionSampler;

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

        public ShaderKey GetShaderKey(RenderPrimitive primitive, MaterialResource material, RenderPass pass)
        {
            return new ShaderKey(this, pass);
        }
    }
}