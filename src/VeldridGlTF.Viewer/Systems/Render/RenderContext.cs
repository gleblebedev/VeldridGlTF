using System.Collections.Generic;
using System.Linq;
using Veldrid;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderContext : IRenderContext
    {
        private readonly VeldridRenderSystem _renderSystem;
        private readonly Model _model;

        public RenderContext(VeldridRenderSystem renderSystem, Model model)
        {
            _renderSystem = renderSystem;
            _model = model;
        }
        public void Invalidate()
        {
            _isValid = false;
        }

        public void Update()
        {
            if (_isValid)
                return;

            _drawCalls.Clear();
            _mesh = _renderSystem.ResolveHandler<IMesh, RenderMesh>(_model.Mesh);
            if (_mesh == null)
            {
                return;
            }

            for (var index = 0; index < _model.Materials.Count && index < _mesh.Primitives.Count; index++)
            {
                var mat = _model.Materials[index];
                var material = _renderSystem.ResolveHandler<IMaterial, RenderMaterial>(mat);
                if (material != null)
                {
                    var indexRange = _mesh.Primitives[index];
                    var shaderKey = new ShaderKey() {VertexLayout = indexRange.Elements};
                    if (material.DiffuseTexture != null && shaderKey.VertexLayout.VertexLayoutDescription.Elements.Any(_=>_.Name == "TEXCOORD_0"))
                    {
                        shaderKey.Flags |= ShaderFlag.DiffuseMap;
                    }
                    var pipeline = _renderSystem.GetPipeline(new PipelineKey()
                    {
                        Shader = shaderKey,
                        PrimitiveTopology = indexRange.PrimitiveTopology
                    });
                    var drawCall = new DrawCall()
                    {
                        Pipeline = pipeline,
                        Material = material
                    };
                    _drawCalls.Add(drawCall);
                }
                else
                {
                    _drawCalls.Add(null);
                }
            }
        }

        public RenderMesh Mesh
        {
            get { return _mesh; }
        }

        public List<DrawCall> DrawCalls
        {
            get { return _drawCalls; }
        }

        private List<DrawCall> _drawCalls = new List<DrawCall>();

        private bool _isValid = false;
        private RenderMesh _mesh;

        public class DrawCall
        {
            public Pipeline Pipeline;

            public RenderMaterial Material;
        }
    }
}