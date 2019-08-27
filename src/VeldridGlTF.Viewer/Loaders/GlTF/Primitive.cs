using System;
using System.Collections.Generic;
using System.Linq;
using SharpGLTF.Schema2;
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

            foreach (var accessorKeyValuePair in meshPrimitive.VertexAccessors)
            {
                var key = accessorKeyValuePair.Key;
                var accessor = accessorKeyValuePair.Value;
                _streams.Add(GeometryStream.Create(key, accessor));
            }

            if (_streams.Any(_ => _.Key == "JOINTS_0" || _.Key == "JOINTS_1"))
            {
                HasSkin = true;
            }

            for (var targetIndex = 0; targetIndex < meshPrimitive.MorphTargetsCount; ++targetIndex)
                foreach (var accessorKeyValuePair in meshPrimitive.GetMorphTargetAccessors(targetIndex))
                {
                    var key = accessorKeyValuePair.Key;
                    var accessor = accessorKeyValuePair.Value;
                    _streams.Add(GeometryStream.Create("TARGET_" + key + targetIndex, accessor));
                }

            Indices = meshPrimitive.GetTriangleIndices().SelectMany(_ => new[] {_.Item1, _.Item2, _.Item3}).ToList();
        }

        public IReadOnlyList<IGeometryStream> Streams => _streams;

        public PrimitiveTopology Topology { get; set; }

        public IReadOnlyCollection<int> Indices { get; }
        public bool HasSkin { get; set; }
    }
}