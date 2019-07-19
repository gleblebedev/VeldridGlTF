using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Systems.Render.Resources;
using VeldridGlTF.Viewer.Systems.Render.Shaders;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Default;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Skybox;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ShaderManager
    {
        private readonly Dictionary<ShaderKey, Shader[]> _compiledShaders = new Dictionary<ShaderKey, Shader[]>();
        private readonly ResourceFactory _factory;
        private readonly Dictionary<string, IShaderFactory> _generators = new Dictionary<string, IShaderFactory>();
        private readonly DefaultShaderFactory _defaultShaderFactory;

        public ShaderManager(ResourceFactory factory)
        {
            _factory = factory;
            _defaultShaderFactory = new DefaultShaderFactory();
            _generators["Default"] = _defaultShaderFactory;
            _generators["Skybox"] = new SkyboxShaderFactory();
        }

        public Shader[] GetShaders(ShaderKey shaderKey, RenderPass pass)
        {
            Shader[] shaders;
            if (_compiledShaders.TryGetValue(shaderKey, out shaders)) return shaders;

            var generatorFactory = shaderKey.Factory;

            var generator = generatorFactory.ResolveGenerator(shaderKey);
            var vertexShader = generator.GetVertexShader();
            var fragmentShader = generator.GetFragmentShader();
            var compiledShader = _factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexShader), "main"),
                new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentShader), "main"));

            _compiledShaders[shaderKey] = compiledShader;

            return compiledShader;
        }

        public ShaderKey GetShaderKey(RenderPrimitive primitive, MaterialResource material, RenderPass renderPass)
        {
            if (!_generators.TryGetValue(material.ShaderName, out var generatorFactory))
            {
                Trace.WriteLine("Shader " + material.ShaderName + " not found. Falling back to default shader.");
                generatorFactory = _defaultShaderFactory;
            }

            return generatorFactory.GetShaderKey(primitive, material, renderPass);
        }
    }
}