using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ShaderManager
    {
        private readonly ResourceFactory _factory;
        private readonly Dictionary<ShaderKey, Shader[]> _compiledShaders = new Dictionary<ShaderKey, Shader[]>();
        private readonly ShaderGenerator _generator;

        public ShaderManager(ResourceFactory factory)
        {
            _factory = factory;
            _generator = new ShaderGenerator();
        }

        public Shader[] GetShaders(ShaderKey shaderKey)
        {
            Shader[] shaders;
            if (_compiledShaders.TryGetValue(shaderKey, out shaders)) return shaders;

            var vertexShader = _generator.GetVertexShader(shaderKey);
            var fragmentShader = _generator.GetFragmentShader(shaderKey);
            var compiledShader = _factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexShader), "main"),
                new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentShader), "main"));

            _compiledShaders[shaderKey] = compiledShader;

            return compiledShader;
        }
    }
}