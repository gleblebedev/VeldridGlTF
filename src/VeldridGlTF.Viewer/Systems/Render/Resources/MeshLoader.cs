using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Utilities;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Data.Geometry;
using VeldridGlTF.Viewer.Resources;
using PrimitiveTopology = Veldrid.PrimitiveTopology;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MeshLoader : IResourceLoader<IMesh>
    {
        private readonly VeldridRenderSystem _renderSystem;

        public MeshLoader(VeldridRenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public async Task<IMesh> LoadAsync(ResourceContext context)
        {
            var geometry = await context.ResolveDependencyAsync<IGeometry>(context.Id);

            return await CreateMesh(context, geometry);
        }

        public async Task<IMesh> CreateMesh(ResourceContext context, IGeometry geometry)
        {
            var indices = new List<ushort>();
            var memory = new VertexBufferStream(1024);
            var primitives = new List<RenderPrimitive>(geometry.Primitives.Count);

            var boundingBox = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));

            foreach (var meshPrimitive in geometry.Primitives)
            {
                var map = new Dictionary<int, ushort>();
                var range = new RenderPrimitive();
                range.Elements = new RenderVertexLayout(meshPrimitive.Streams.Select(GetVertexElementDescription));
                range.PrimitiveTopology = GetPrimitiveTopology(meshPrimitive.Topology);
                range.DataOffset = (uint) memory.Position;
                range.Start = (uint) indices.Count;
                ushort numVertices = 0;
                foreach (var index in meshPrimitive.Indices)
                    indices.Add(CopyVertex(index, map, memory, meshPrimitive.Streams, ref numVertices));

                range.Length = (uint) (indices.Count - range.Start);
                primitives.Add(range);

                var positionStream = meshPrimitive.Streams.FirstOrDefault(_ => _.Key == "POSITION");
                if (positionStream != null)
                {
                    var positions = positionStream.AsVector3();
                    Vector3 min = new Vector3(float.MaxValue);
                    Vector3 max = new Vector3(float.MinValue);

                    for (int i = 0; i < numVertices; i++)
                    {
                        Vector3 pos = positions[i];

                        if (min.X > pos.X) min.X = pos.X;
                        if (max.X < pos.X) max.X = pos.X;

                        if (min.Y > pos.Y) min.Y = pos.Y;
                        if (max.Y < pos.Y) max.Y = pos.Y;

                        if (min.Z > pos.Z) min.Z = pos.Z;
                        if (max.Z < pos.Z) max.Z = pos.Z;
                    }
                    boundingBox = BoundingBox.Combine(boundingBox, new BoundingBox(min, max));
                }
            }

            var _vertices = memory.ToArray();
            var renderContext = await context.ResolveDependencyAsync(_renderSystem.RenderContext);

            var vertexBuffer =
                renderContext.Factory.CreateBuffer(new BufferDescription(
                    (uint) _vertices.Length, BufferUsage.VertexBuffer));
            renderContext.Device.UpdateBuffer(vertexBuffer, 0, _vertices);

            var indexBuffer =
                renderContext.Factory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint) indices.Count,
                    BufferUsage.IndexBuffer));
            renderContext.Device.UpdateBuffer(indexBuffer, 0, indices.ToArray());

            return new RenderMesh(context.Id, vertexBuffer, indexBuffer, boundingBox, primitives);
        }

        private ushort CopyVertex(int originalIndex, Dictionary<int, ushort> map, VertexBufferStream vbStream,
            IEnumerable<IGeometryStream> accessors, ref ushort numVertices)
        {
            ushort index;
            if (map.TryGetValue(originalIndex, out index))
                return index;
            index = numVertices;
            ++numVertices;

            map.Add(originalIndex, index);
            foreach (var accessor in accessors) accessor.Write(originalIndex, vbStream);
            return index;
        }

        private PrimitiveTopology GetPrimitiveTopology(Data.Geometry.PrimitiveTopology meshPrimitiveTopology)
        {
            return (PrimitiveTopology) (int) meshPrimitiveTopology;
        }

        private VertexElementDescription GetVertexElementDescription(IGeometryStream geometryStream)
        {
            return new VertexElementDescription(geometryStream.Key, GetFormat(geometryStream.Format),
                VertexElementSemantic.TextureCoordinate);
        }

        private VertexElementFormat GetFormat(GeometryStreamFormat geometryStreamFormat)
        {
            return (VertexElementFormat) (int) geometryStreamFormat;
        }
    }
}