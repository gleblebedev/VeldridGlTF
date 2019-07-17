using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Data.Geometry;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render.Resources;
using PrimitiveTopology = VeldridGlTF.Viewer.Data.Geometry.PrimitiveTopology;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class SkyboxGeometry : IGeometry
    {
        public static readonly IResourceHandler<IGeometry> Handler =
            new ManualResourceHandler<IGeometry>(ResourceId.Null, new SkyboxGeometry());

        private static readonly IReadOnlyList<IGeometryStream> _streams = new[]
        {
            new Vector3GeometryStream("POSITION", new[]
            {
                new Vector3(-1f, 1f, -1f), new Vector3(1f, 1f, -1f),
                new Vector3(1f, 1f, 1f), new Vector3(-1f, 1f, 1f),
                new Vector3(-1f, -1f, 1f), new Vector3(1f, -1f, 1f),
                new Vector3(1f, -1f, -1f), new Vector3(-1f, -1f, -1f)
            })
        };

        private static readonly int[] _indices =
        {
            0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 0, 3, 4, 0, 4, 7, 2, 1, 6, 2, 6, 5, 1, 0, 7, 1, 7, 6, 3, 2, 5, 3, 5, 4
        };

        private static readonly IReadOnlyList<IGeometryPrimitive> _primitives = new[] {new SkyboxPrimitive()};

        private Texture _cubemap;
        private TextureView _cubemapView;
        private Mesh _mesh;
        private StaticModel _model;
        private Pipeline _pipeline;

        public IReadOnlyList<IGeometryPrimitive> Primitives => _primitives;

        internal class SkyboxPrimitive : IGeometryPrimitive
        {
            public IReadOnlyList<IGeometryStream> Streams => _streams;

            public PrimitiveTopology Topology => PrimitiveTopology.TriangleList;

            public IReadOnlyCollection<int> Indices => _indices;
        }
    }
}