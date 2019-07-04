using System.Collections.Generic;
using System.Linq;
using Veldrid;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderContext : IRenderContext
    {
        private readonly Model _model;
        private readonly VeldridRenderSystem _renderSystem;

        private bool _isValid;
        private RenderMesh _mesh;

        public RenderContext(VeldridRenderSystem renderSystem, Model model)
        {
            _renderSystem = renderSystem;
            _model = model;
        }

        public RenderMesh Mesh => _mesh;

        public List<DrawCall> DrawCalls { get; } = new List<DrawCall>();

        public void Invalidate()
        {
            _isValid = false;
        }

        public void Update()
        {
            if (_isValid)
                return;

            DrawCalls.Clear();

            if (!_model.Mesh.TryGetAs(out _mesh) || _mesh == null) return;

            for (var index = 0; index < _model.Materials.Count && index < _mesh.Primitives.Count; index++)
            {
                var mat = _model.Materials[index];

                MaterialResource material;
                if (mat.TryGetAs(out material))
                {
                    var indexRange = _mesh.Primitives[index];
                    var shaderKey = new ShaderKey {VertexLayout = indexRange.Elements};
                    if (material.DiffuseTexture != null &&
                        shaderKey.VertexLayout.VertexLayoutDescription.Elements.Any(_ => _.Name == "TEXCOORD_0"))
                        shaderKey.Flags |= ShaderFlag.DiffuseMap;
                    var pipeline = _renderSystem.GetPipeline(new PipelineKey
                    {
                        Shader = shaderKey,
                        PrimitiveTopology = indexRange.PrimitiveTopology
                    });
                    var drawCall = new DrawCall
                    {
                        Pipeline = pipeline,
                        Material = material
                    };
                    DrawCalls.Add(drawCall);
                }
                else
                {
                    DrawCalls.Add(null);
                }
            }
        }

        public class DrawCall
        {
            public MaterialResource Material;
            public Pipeline Pipeline;
        }
    }
}