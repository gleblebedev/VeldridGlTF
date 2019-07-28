﻿using System.Collections.Generic;
using System.Diagnostics;
using Veldrid;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Systems.Render.Resources;
using VeldridGlTF.Viewer.Systems.Render.Shaders;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Default;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Skybox;
using ShaderDescription = Veldrid.ShaderDescription;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ShaderManager
    {
        private readonly Dictionary<ShaderKey, ShaderAndLayout> _compiledShaders = new Dictionary<ShaderKey, ShaderAndLayout>();
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

        public ShaderAndLayout GetShaders(ShaderKey shaderKey, RenderPass pass)
        {
            ShaderAndLayout shaderAndLayout;
            if (_compiledShaders.TryGetValue(shaderKey, out shaderAndLayout)) return shaderAndLayout;

            var generatorFactory = shaderKey.Factory;

            var generator = generatorFactory.ResolveGenerator(shaderKey);
            (var vertexShader, var fragmentShader) = SpirvReflection.CompileGlslToSpirv(generator.GetVertexShader(), generator.GetFragmentShader());

            var compiledShader = _factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, vertexShader.SpirvBytes, "main"),
                new ShaderDescription(ShaderStages.Fragment, fragmentShader.SpirvBytes, "main"));

            shaderAndLayout = new ShaderAndLayout();
            shaderAndLayout.Shaders = compiledShader;
            shaderAndLayout.Layouts = SpirvCompilationResultEx.Merge(vertexShader.Layouts, fragmentShader.Layouts);
            _compiledShaders.Add(shaderKey, shaderAndLayout);
            return shaderAndLayout;
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