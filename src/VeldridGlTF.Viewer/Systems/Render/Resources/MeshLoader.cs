using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veldrid;
using VeldridGlTF.Viewer.Data;
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

        public async Task<IMesh> LoadAsync(ResourceManager manager, ResourceId id)
        {
            var geometry = await manager.Resolve<IGeometry>(id).GetAsyncOrDefault();
            if (geometry == null) return null;

            var indices = new List<ushort>();
            var memory = new VertexBufferStream(1024);
            var Primitives = new List<RenderPrimitive>(geometry.Primitives.Count);
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
                Primitives.Add(range);
            }

            var _vertices = memory.ToArray();
            var graphicsDevice = await _renderSystem.GraphicsDevice;
            var factory = await _renderSystem.ResourceFactory;

            var _vertexBuffer =
                factory.CreateBuffer(new BufferDescription(
                    (uint) _vertices.Length, BufferUsage.VertexBuffer));
            graphicsDevice.UpdateBuffer(_vertexBuffer, 0, _vertices);

            var _indexBuffer =
                factory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint) indices.Count,
                    BufferUsage.IndexBuffer));
            graphicsDevice.UpdateBuffer(_indexBuffer, 0, indices.ToArray());

            return new RenderMesh(id, _vertexBuffer, _indexBuffer, Primitives);
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

        private PrimitiveTopology GetPrimitiveTopology(Data.PrimitiveTopology meshPrimitiveTopology)
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