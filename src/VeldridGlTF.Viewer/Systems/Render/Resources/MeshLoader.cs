using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veldrid;
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
            }

            var _vertices = memory.ToArray();
            var graphicsDevice = await context.ResolveDependencyAsync(_renderSystem.GraphicsDevice);
            var factory = await context.ResolveDependencyAsync(_renderSystem.ResourceFactory);

            var vertexBuffer =
                factory.CreateBuffer(new BufferDescription(
                    (uint) _vertices.Length, BufferUsage.VertexBuffer));
            graphicsDevice.UpdateBuffer(vertexBuffer, 0, _vertices);

            var indexBuffer =
                factory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint) indices.Count,
                    BufferUsage.IndexBuffer));
            graphicsDevice.UpdateBuffer(indexBuffer, 0, indices.ToArray());

            return new RenderMesh(context.Id, vertexBuffer, indexBuffer, primitives);
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