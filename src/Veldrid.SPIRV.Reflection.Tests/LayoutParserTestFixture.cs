using System;
using System.Linq;
using NUnit.Framework;

namespace Veldrid.SPIRV
{
    [TestFixture]
    public class LayoutParserTestFixture
    {
        static readonly GlslCompileOptions glslCompileOptions = new GlslCompileOptions(true);

        [Test]
        [TestCase("float", "vec4(DataBlock.POSITION)")]
        [TestCase("vec2", "vec4(DataBlock.POSITION,DataBlock.POSITION)")]
        [TestCase("vec3", "vec4(DataBlock.POSITION,1)")]
        [TestCase("vec4", "DataBlock.POSITION")]
        [TestCase("mat4", "DataBlock.POSITION[0]")]
        public void FloatUniform(string typeName, string makeVec4)
        {
            #region Shader Source Code

            var sourceCode = @"
#version 450

struct DataBlockType
{
    " + typeName + @" POSITION;
};

layout(set = 1, binding = 2) uniform DataBlock2
{
    DataBlockType DataBlock;
};

void main()
{
    gl_Position = " + makeVec4 + @";
}
";

            #endregion

            var bytecode = SpirvCompilation
                .CompileGlslToSpirv(sourceCode, "shader.glsl", ShaderStages.Vertex, glslCompileOptions)
                .SpirvBytes;

            var reflection = SpirvReflection.Parse(bytecode, ShaderStages.Vertex);
#if DEBUG
            foreach (var uniform in reflection.SelectMany(_=>_.Elements))
            {
                Console.WriteLine(uniform);
            }
#endif
            Assert.AreEqual("DataBlock2", reflection[1].Elements[2].Name);
        }


        [Test]
        public void SampleUniform()
        {
            #region Shader Source Code

            var sourceCode = @"
#version 450

layout(set = 3, binding = 1) uniform textureCube SurfaceTexture;
layout(set = 3, binding = 2) uniform sampler SurfaceSampler;

void main()
{
    gl_Position = texture(samplerCube(SurfaceTexture, SurfaceSampler), vec3(1,2,3));
}
";

            #endregion

            var bytecode = SpirvCompilation
                .CompileGlslToSpirv(sourceCode, "shader.glsl", ShaderStages.Vertex, glslCompileOptions)
                .SpirvBytes;

            var reflection = SpirvReflection.Parse(bytecode, ShaderStages.Vertex);
#if DEBUG
            foreach (var uniform in reflection.SelectMany(_ => _.Elements))
            {
                Console.WriteLine(uniform);
            }
#endif
            Assert.AreEqual("SurfaceTexture", reflection[3].Elements[1].Name);
            Assert.AreEqual("SurfaceSampler", reflection[3].Elements[2].Name);
            Assert.AreEqual(ResourceKind.TextureReadOnly, reflection[3].Elements[1].Kind);
            Assert.AreEqual(ResourceKind.Sampler, reflection[3].Elements[2].Kind);

        }

    }
}