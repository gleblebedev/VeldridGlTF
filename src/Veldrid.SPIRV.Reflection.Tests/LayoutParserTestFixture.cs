using System;
using NUnit.Framework;

namespace Veldrid.SPIRV
{
    [TestFixture]
    public class LayoutParserTestFixture
    {
        static readonly GlslCompileOptions glslCompileOptions = new GlslCompileOptions(true);
        [Test]
        public void FloatParameter()
        {
            #region Shader Source Code

            var sourceCode = @"
#version 450

layout(location = 1) in float POSITION;

void main()
{
    gl_Position = vec4(POSITION);
}
";

            #endregion

            var bytecode = SpirvCompilation
                .CompileGlslToSpirv(sourceCode, "shader.glsl", ShaderStages.Vertex, glslCompileOptions)
                .SpirvBytes;

            var reflection = SpirvReflection.Parse(bytecode);

#if DEBUG
            foreach (var input in reflection.Inputs)
            {
                Console.WriteLine(input);
            }
#endif

            Assert.AreEqual(1, reflection.Inputs.Count);
            var reflectionInput = reflection.Inputs[0];
            Assert.AreEqual(1, reflectionInput.Location.Value);
            Assert.AreEqual("float", reflectionInput.TypeName);
            Assert.AreEqual(4, reflectionInput.Size);
        }

        [Test]
        [TestCase("float", "vec4(POSITION)")]
        [TestCase("vec2", "vec4(POSITION,POSITION)")]
        [TestCase("vec3", "vec4(POSITION,1)")]
        [TestCase("vec4", "POSITION")]
        [TestCase("mat4", "POSITION[0]")]
        public void FloatUniform(string typeName, string makeVec4)
        {
            #region Shader Source Code

            var sourceCode = @"
#version 450

layout(set = 1, binding = 2) uniform DataBlock
{
    " + typeName + @" POSITION;
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

            var reflection = SpirvReflection.Parse(bytecode);
#if DEBUG
            foreach (var uniform in reflection.Uniforms)
            {
                Console.WriteLine(uniform);
            }
#endif
            Assert.AreEqual(1, reflection.Uniforms.Count);
            var reflectionUniform = reflection.Uniforms[0];
            Assert.AreEqual(1, reflectionUniform.Set.Value);
            Assert.AreEqual(2, reflectionUniform.Binding.Value);
            Assert.AreEqual(typeName, reflectionUniform.TypeName);
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

            var reflection = SpirvReflection.Parse(bytecode);
#if DEBUG
            foreach (var uniform in reflection.Uniforms)
            {
                Console.WriteLine(uniform);
            }
#endif
            Assert.AreEqual(2, reflection.Uniforms.Count);
            var reflectionUniform1 = reflection.Uniforms[0];
            Assert.AreEqual(3, reflectionUniform1.Set.Value);
            Assert.AreEqual(1, reflectionUniform1.Binding.Value);
            var reflectionUniform2 = reflection.Uniforms[1];
            Assert.AreEqual(3, reflectionUniform2.Set.Value);
            Assert.AreEqual(2, reflectionUniform2.Binding.Value);
        }

    }
}