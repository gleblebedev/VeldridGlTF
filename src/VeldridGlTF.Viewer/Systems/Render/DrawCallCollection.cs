using System.Collections.Generic;
using SharpGLTF.Geometry;
using Veldrid;
using Veldrid.Utilities;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class DrawCallCollection
    {
        public DrawCallCollection(Mesh mesh, int capacity, IEnumerable<DrawCall> drawCalls)
        {
            DrawCalls = new List<DrawCall>(capacity);
            IndexBuffer = mesh.IndexBuffer;
            VertexBuffer = mesh.VertexBuffer;
            BoundingBox = mesh.BoundingBox;
            MorphWeights = mesh.DefaultMorphWeights;
            foreach (var drawCall in drawCalls)
                if (drawCall != null)
                    DrawCalls.Add(drawCall);
        }

        public IReadOnlyList<float> MorphWeights { get; set; }

        public List<DrawCall> DrawCalls { get; set; }

        public DeviceBuffer VertexBuffer { get; set; }

        public DeviceBuffer IndexBuffer { get; set; }

        public BoundingBox BoundingBox { get; set; }
    }
}