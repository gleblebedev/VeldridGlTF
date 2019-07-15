using System;
using System.Collections.Generic;
using System.Linq;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Data.Geometry;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class Primitive : IGeometryPrimitive
    {
        private readonly List<IGeometryStream> _streams;

        public Primitive(MeshPrimitive meshPrimitive)
        {
            switch (meshPrimitive.DrawPrimitiveType)
            {
                case PrimitiveType.TRIANGLES:
                    Topology = PrimitiveTopology.TriangleList;
                    break;
                case PrimitiveType.TRIANGLE_STRIP:
                    Topology = PrimitiveTopology.TriangleList;
                    break;
                case PrimitiveType.TRIANGLE_FAN:
                    Topology = PrimitiveTopology.TriangleList;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _streams = new List<IGeometryStream>(meshPrimitive.VertexAccessors.Count);
            foreach (var accessor in meshPrimitive.VertexAccessors)
                _streams.Add(GeometryStream.Create(accessor.Key, accessor.Value));

            Indices = meshPrimitive.GetTriangleIndices().SelectMany(_ => new[] {_.Item1, _.Item2, _.Item3}).ToList();
        }

        public IReadOnlyList<IGeometryStream> Streams => _streams;

        public PrimitiveTopology Topology { get; set; }

        public IReadOnlyCollection<int> Indices { get; }
    }
}