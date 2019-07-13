﻿using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Default;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ShaderManager
    {
        private readonly Dictionary<ShaderKey, Shader[]> _compiledShaders = new Dictionary<ShaderKey, Shader[]>();
        private readonly ResourceFactory _factory;
        private readonly DefaultShaderFactory _generator;

        public ShaderManager(ResourceFactory factory)
        {
            _factory = factory;
            _generator = new DefaultShaderFactory();
        }

        public Shader[] GetShaders(ShaderKey shaderKey)
        {
            Shader[] shaders;
            if (_compiledShaders.TryGetValue(shaderKey, out shaders)) return shaders;

            var generator = _generator.ResolveGenerator(shaderKey);
            var vertexShader = generator.GetVertexShader();
            var fragmentShader = generator.GetFragmentShader();
            var compiledShader = _factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexShader), "main"),
                new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentShader), "main"));

            _compiledShaders[shaderKey] = compiledShader;

            return compiledShader;
        }
    }
}