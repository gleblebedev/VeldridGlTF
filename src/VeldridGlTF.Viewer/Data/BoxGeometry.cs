using System.Collections.Generic;
using System.Numerics;
using VeldridGlTF.Viewer.Data.Geometry;

namespace VeldridGlTF.Viewer.Data
{
    public class BoxGeometry : IGeometry

    {
        private static readonly IReadOnlyList<float> _morphWeights = new float[0];

        public IReadOnlyList<IGeometryPrimitive> Primitives { get; } = new[] {new BoxGeometryPrimitive()};

        public IReadOnlyList<float> MorphWeights => _morphWeights;
        public uint JointCount { get; }

        internal class BoxGeometryPrimitive : IGeometryPrimitive
        {
            public BoxGeometryPrimitive()
            {
                Streams = new List<IGeometryStream>
                {
                    AbstractGeometryStream.Create("NORMAL",
                        new[]
                        {
                            new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0),
                            new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1),
                            new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0),
                            new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1),
                            new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0),
                            new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0)
                        }),
                    AbstractGeometryStream.Create("POSITION",
                        new[]
                        {
                            new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f),
                            new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
                            new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
                            new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f),
                            new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f),
                            new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f),
                            new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
                            new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f),
                            new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f),
                            new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
                            new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
                            new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, -0.5f)
                        })
                };
                Indices = new[]
                {
                    0, 1, 2, 3, 2, 1, 4, 5, 6, 7, 6, 5, 8, 9, 10, 11, 10, 9, 12, 13, 14, 15, 14, 13, 16, 17, 18, 19, 18,
                    17, 20, 21, 22, 23, 22, 21
                };
            }

            public IReadOnlyList<IGeometryStream> Streams { get; }

            public PrimitiveTopology Topology => PrimitiveTopology.TriangleList;

            public IReadOnlyCollection<int> Indices { get; }

            public bool HasSkin { get; }
        }
    }
}