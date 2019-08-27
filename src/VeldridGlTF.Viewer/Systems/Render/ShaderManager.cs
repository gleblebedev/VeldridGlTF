using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SharpDX.Text;
using Veldrid;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render.Resources;
using VeldridGlTF.Viewer.Systems.Render.Shaders;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Default;
using VeldridGlTF.Viewer.Systems.Render.Shaders.PBR;
using VeldridGlTF.Viewer.Systems.Render.Shaders.Skybox;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ShaderManager
    {
        private readonly LazyAsyncCollection<ShaderKey, ShaderAndLayout> _compiledShaders;
        private readonly IShaderFactory _defaultShaderFactory;
        private readonly ResourceFactory _factory;
        private readonly Dictionary<string, IShaderFactory> _generators = new Dictionary<string, IShaderFactory>();
        private readonly IShaderFactory _pbrShaderFactory;

        public ShaderManager(ResourceFactory factory)
        {
            _factory = factory;
            _defaultShaderFactory = new DefaultShaderFactory();
            _pbrShaderFactory = new PBRShaderFactory();
            _generators["Default"] = _pbrShaderFactory;
            //_generators["Default"] = _defaultShaderFactory;
            _generators["Skybox"] = new SkyboxShaderFactory();
            _compiledShaders = new LazyAsyncCollection<ShaderKey, ShaderAndLayout>(CompileShader);
        }

        private ShaderAndLayout CompileShader(ShaderKey shaderKey)
        {
            var generatorFactory = shaderKey.Factory;

            var generator = generatorFactory.ResolveGenerator(shaderKey);
            var vertexShaderText = generator.GetVertexShader();
            var fragmentShaderText = generator.GetFragmentShader();
            var (vertexShader, fragmentShader) = SpirvReflection2.CompileGlslToSpirv(vertexShaderText,
                fragmentShaderText, shaderKey.LayoutNameResolver);

            var compiledShader = _factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexShaderText), "main"),
                new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentShaderText), "main"));

            var shaderAndLayout = new ShaderAndLayout();
            shaderAndLayout.Shaders = compiledShader;
            shaderAndLayout.Layouts = SpirvCompilationResultEx.Merge(vertexShader.Layouts, fragmentShader.Layouts);
            return shaderAndLayout;
        }

        public Task<ShaderAndLayout> GetShaders(ShaderKey shaderKey)
        {
            return _compiledShaders[shaderKey];
        }

        public ShaderKey GetShaderKey(RenderPrimitive primitive, MaterialResource material,
            ILayoutNameResolver renderPass)
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