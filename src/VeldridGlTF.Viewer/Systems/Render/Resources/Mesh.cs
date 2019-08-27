using System;
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
    public class Mesh : AbstractResource, IMesh
    {
        public readonly List<RenderPrimitive> _primitives;
        public DeviceBuffer IndexBuffer;
        public DeviceBuffer VertexBuffer;
        public uint JointCount;
        private Mesh(ResourceId id, DeviceBuffer vertexBuffer, DeviceBuffer indexBuffer, BoundingBox boundingBox,
            List<RenderPrimitive> primitives) : base(id)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            _primitives = primitives;
            BoundingBox = boundingBox;
        }

        public List<RenderPrimitive> Primitives => _primitives;

        public BoundingBox BoundingBox { get; set; }

        public IReadOnlyList<float> DefaultMorphWeights { get; set; }

        public static async Task<IMesh> Create(VeldridRenderSystem renderSystem, ResourceContext context,
            IGeometry geometry)
        {
            if (geometry == null)
                return null;
            if (renderSystem == null)
                throw new ArgumentNullException(nameof(renderSystem));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var indices = new List<ushort>();
            var memory = new VertexBufferStream(1024);
            var primitives = new List<RenderPrimitive>(geometry.Primitives.Count);

            var boundingBox = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));

            foreach (var meshPrimitive in geometry.Primitives)
            {
                var map = new Dictionary<int, ushort>();
                var range = new RenderPrimitive();

                range.JointCount = meshPrimitive.HasSkin ? geometry.JointCount : 0;
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
                    var min = new Vector3(float.MaxValue);
                    var max = new Vector3(float.MinValue);

                    for (var i = 0; i < numVertices; i++)
                    {
                        var pos = positions[i];

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

            var vertices = memory.ToArray();
            var renderContext = await context.ResolveDependencyAsync(renderSystem.RenderContext);

            var vertexBuffer =
                renderContext.Factory.CreateBuffer(new BufferDescription(
                    (uint) vertices.Length, BufferUsage.VertexBuffer));
            renderContext.Device.UpdateBuffer(vertexBuffer, 0, vertices);

            var indexBuffer =
                renderContext.Factory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint) indices.Count,
                    BufferUsage.IndexBuffer));
            renderContext.Device.UpdateBuffer(indexBuffer, 0, indices.ToArray());

            var mesh = new Mesh(context.Id, vertexBuffer, indexBuffer, boundingBox, primitives);
            if (geometry.JointCount > 0)
            {
                uint jointCount = 1;
                while (jointCount < geometry.JointCount)
                {
                    jointCount *= 2;
                }
                mesh.JointCount = jointCount;
            }
            mesh.DefaultMorphWeights = geometry.MorphWeights;
            return mesh;
        }

        public static async Task<IMesh> Create(VeldridRenderSystem renderSystem, ResourceContext context,
            IResourceHandler<IGeometry> geometryHandler)
        {
            if (geometryHandler == null)
                return null;
            if (renderSystem == null)
                throw new ArgumentNullException(nameof(renderSystem));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var geometry = await context.ResolveDependencyAsync(geometryHandler);
            return await Create(renderSystem, context, geometry);
        }

        private static PrimitiveTopology GetPrimitiveTopology(Data.Geometry.PrimitiveTopology meshPrimitiveTopology)
        {
            return (PrimitiveTopology) (int) meshPrimitiveTopology;
        }

        private static ushort CopyVertex(int originalIndex, Dictionary<int, ushort> map, VertexBufferStream vbStream,
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

        private static VertexElementDescription GetVertexElementDescription(IGeometryStream geometryStream)
        {
            //TODO: Everything is VertexElementSemantic.TextureCoordinate because of SPIRV shader translation.
            return new VertexElementDescription(geometryStream.Key, GetFormat(geometryStream.Format),
                VertexElementSemantic.TextureCoordinate);
        }

        private static VertexElementFormat GetFormat(GeometryStreamFormat geometryStreamFormat)
        {
            return (VertexElementFormat) (int) geometryStreamFormat;
        }
    }
}