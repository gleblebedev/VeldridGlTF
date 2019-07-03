using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;
using Veldrid;
using Veldrid.Utilities;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Systems.Render.ElementAccessors;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderMesh : IMesh
    {
        private readonly ushort[] _indices;
        private readonly byte[] _vertices;

        public DeviceBuffer _indexBuffer;
        public DeviceBuffer _vertexBuffer;
        public List<RenderPrimitive> Primitives = new List<RenderPrimitive>();
        private BoundingBox _bbox;

        public RenderMesh(Mesh mesh)
        {
            var indices = new List<ushort>();
            var memory = new VertexBufferStream(1024);

            BoundingBox bbox = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
            foreach (var meshPrimitive in mesh.Primitives)
            {
                var elements = new List<AbstractElementAccessor>();
                foreach (var accessor in meshPrimitive.VertexAccessors)
                {
                    var elementAccessor = AbstractElementAccessor.GetElementAccessor(accessor.Key, accessor.Value);
                    elements.Add(elementAccessor);
                }

                var positions = meshPrimitive.GetVertices("POSITION").AsVector3Array();
                bbox = BoundingBox.Combine(bbox, BoundingBox.CreateFromVertices(positions.ToArray()));

                var map = new Dictionary<int, ushort>();
                var range = new RenderPrimitive();
                range.Elements = new RenderVertexLayout(elements.Select(_=>_.VertexElementDescription));
                range.PrimitiveTopology = PrimitiveTopology.TriangleList;
                range.DataOffset = (uint)memory.Position;
                range.Start = (uint)indices.Count;
                ushort numVertices = 0;
                foreach (var face in meshPrimitive.GetTriangleIndices())
                {
                    indices.Add(CopyVertex(face.Item1, map, memory, elements, ref numVertices));
                    indices.Add(CopyVertex(face.Item2, map, memory, elements, ref numVertices));
                    indices.Add(CopyVertex(face.Item3, map, memory, elements, ref numVertices));
                }

                range.Length = (uint) (indices.Count-range.Start);
                Primitives.Add(range);
            }

            _bbox = bbox;
            _vertices = memory.ToArray();
            _indices = indices.ToArray();
        }

        private ushort CopyVertex(int originalIndex, Dictionary<int, ushort> map, VertexBufferStream vbStream, IEnumerable<AbstractElementAccessor> accessors, ref ushort numVertices)
        {
            ushort index;
            if (map.TryGetValue(originalIndex, out index))
                return index;
            index = numVertices;
            ++numVertices;

            map.Add(originalIndex, index);
            foreach (var accessor in accessors)
            {
                accessor.Write((int)originalIndex, vbStream);
            }
            return index;
        }

        public void EnsureResources(GraphicsDevice graphicsDevice, ResourceFactory factory)
        {
            if (_vertexBuffer == null)
            {
                _vertexBuffer =
                    factory.CreateBuffer(new BufferDescription(
                        (uint) (VertexPositionTexture.SizeInBytes * _vertices.Length), BufferUsage.VertexBuffer));
                graphicsDevice.UpdateBuffer(_vertexBuffer, 0, _vertices);
            }

            if (_indexBuffer == null)
            {
                _indexBuffer =
                    factory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint) _indices.Length,
                        BufferUsage.IndexBuffer));
                graphicsDevice.UpdateBuffer(_indexBuffer, 0, _indices);
            }
        }

        //private static VertexPositionTexture[] GetCubeVertices()
        //{
        //    const float halfSize = 0.02f;
        //    VertexPositionTexture[] vertices =
        //    {
        //        // Top
        //        new VertexPositionTexture(new Vector3(-halfSize, +halfSize, -halfSize), new Vector2(0, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, +halfSize, -halfSize), new Vector2(1, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, +halfSize, +halfSize), new Vector2(1, 1)),
        //        new VertexPositionTexture(new Vector3(-halfSize, +halfSize, +halfSize), new Vector2(0, 1)),
        //        // Bottom                                                             
        //        new VertexPositionTexture(new Vector3(-halfSize, -halfSize, +halfSize), new Vector2(0, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, -halfSize, +halfSize), new Vector2(1, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, -halfSize, -halfSize), new Vector2(1, 1)),
        //        new VertexPositionTexture(new Vector3(-halfSize, -halfSize, -halfSize), new Vector2(0, 1)),
        //        // Left                                                               
        //        new VertexPositionTexture(new Vector3(-halfSize, +halfSize, -halfSize), new Vector2(0, 0)),
        //        new VertexPositionTexture(new Vector3(-halfSize, +halfSize, +halfSize), new Vector2(1, 0)),
        //        new VertexPositionTexture(new Vector3(-halfSize, -halfSize, +halfSize), new Vector2(1, 1)),
        //        new VertexPositionTexture(new Vector3(-halfSize, -halfSize, -halfSize), new Vector2(0, 1)),
        //        // Right                                                              
        //        new VertexPositionTexture(new Vector3(+halfSize, +halfSize, +halfSize), new Vector2(0, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, +halfSize, -halfSize), new Vector2(1, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, -halfSize, -halfSize), new Vector2(1, 1)),
        //        new VertexPositionTexture(new Vector3(+halfSize, -halfSize, +halfSize), new Vector2(0, 1)),
        //        // Back                                                               
        //        new VertexPositionTexture(new Vector3(+halfSize, +halfSize, -halfSize), new Vector2(0, 0)),
        //        new VertexPositionTexture(new Vector3(-halfSize, +halfSize, -halfSize), new Vector2(1, 0)),
        //        new VertexPositionTexture(new Vector3(-halfSize, -halfSize, -halfSize), new Vector2(1, 1)),
        //        new VertexPositionTexture(new Vector3(+halfSize, -halfSize, -halfSize), new Vector2(0, 1)),
        //        // Front                                                              
        //        new VertexPositionTexture(new Vector3(-halfSize, +halfSize, +halfSize), new Vector2(0, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, +halfSize, +halfSize), new Vector2(1, 0)),
        //        new VertexPositionTexture(new Vector3(+halfSize, -halfSize, +halfSize), new Vector2(1, 1)),
        //        new VertexPositionTexture(new Vector3(-halfSize, -halfSize, +halfSize), new Vector2(0, 1))
        //    };

        //    return vertices;
        //}

        //private static ushort[] GetCubeIndices()
        //{
        //    ushort[] indices =
        //    {
        //        0, 1, 2, 0, 2, 3,
        //        4, 5, 6, 4, 6, 7,
        //        8, 9, 10, 8, 10, 11,
        //        12, 13, 14, 12, 14, 15,
        //        16, 17, 18, 16, 18, 19,
        //        20, 21, 22, 20, 22, 23
        //    };

        //    return indices;
        //}
    }
}