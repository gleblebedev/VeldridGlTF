using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;
using Veldrid;
using VeldridGlTF.Viewer.Data;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderMesh : IMesh
    {
        public DeviceBuffer _indexBuffer;
        private readonly ushort[] _indices;
        public DeviceBuffer _vertexBuffer;
        private readonly VertexPositionTexture[] _vertices;

        public RenderMesh(Mesh mesh)
        {
            var vertices = new List<VertexPositionTexture>();
            var indices = new List<ushort>();

            foreach (var meshPrimitive in mesh.Primitives)
            {
                var positions = meshPrimitive.GetVertices("POSITION").AsVector3Array();
                var normals = GetMemoryAccessor(meshPrimitive.GetVertexAccessor("NORMAL"))?.AsVector3Array();
                var texCoords0 = GetMemoryAccessor(meshPrimitive.GetVertexAccessor("TEXCOORD_0"))?.AsVector2Array();
                var map = new Dictionary<int, ushort>();
                foreach (var face in meshPrimitive.GetTriangleIndices())
                {
                    indices.Add(CopyVertex(face.Item1, map, vertices, positions, normals, texCoords0));
                    indices.Add(CopyVertex(face.Item2, map, vertices, positions, normals, texCoords0));
                    indices.Add(CopyVertex(face.Item3, map, vertices, positions, normals, texCoords0));
                }
            }

            _vertices = vertices.ToArray();
            _indices = indices.ToArray();
            IndexCount = (uint) _indices.Length;
        }

        public uint IndexCount { get; set; }

        private ushort CopyVertex(int originalIndex, Dictionary<int, ushort> map, List<VertexPositionTexture> vertices,
            Vector3Array positions, Vector3Array? normals, Vector2Array? texCoords0)
        {
            ushort index;
            if (map.TryGetValue(originalIndex, out index))
                return index;
            index = (ushort) vertices.Count;
            map.Add(originalIndex, index);
            var vertexPositionTexture = new VertexPositionTexture();
            var pos = positions[originalIndex];
            vertexPositionTexture.PosX = pos.X;
            vertexPositionTexture.PosY = pos.Y;
            vertexPositionTexture.PosZ = pos.Z;
            if (texCoords0 != null)
            {
                var uv = texCoords0.Value[originalIndex];
                vertexPositionTexture.TexU = uv.X;
                vertexPositionTexture.TexV = uv.Y;
            }

            vertices.Add(vertexPositionTexture);
            return index;
        }

        internal MemoryAccessor GetMemoryAccessor(Accessor accessor)
        {
            if (accessor == null)
                return null;
            var view = accessor.SourceBufferView;
            var info = new MemoryAccessInfo(null, accessor.ByteOffset, accessor.Count, view.ByteStride,
                accessor.Dimensions, accessor.Encoding, accessor.Normalized);
            return new MemoryAccessor(view.Content, info);
        }

        public void CreateResources(GraphicsDevice graphicsDevice, ResourceFactory factory)
        {
            _vertexBuffer =
                factory.CreateBuffer(new BufferDescription(
                    (uint) (VertexPositionTexture.SizeInBytes * _vertices.Length), BufferUsage.VertexBuffer));
            graphicsDevice.UpdateBuffer(_vertexBuffer, 0, _vertices);

            _indexBuffer =
                factory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint) _indices.Length,
                    BufferUsage.IndexBuffer));
            graphicsDevice.UpdateBuffer(_indexBuffer, 0, _indices);
        }

        private static VertexPositionTexture[] GetCubeVertices()
        {
            const float halfSize = 0.02f;
            VertexPositionTexture[] vertices =
            {
                // Top
                new VertexPositionTexture(new Vector3(-halfSize, +halfSize, -halfSize), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, +halfSize, -halfSize), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, +halfSize, +halfSize), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-halfSize, +halfSize, +halfSize), new Vector2(0, 1)),
                // Bottom                                                             
                new VertexPositionTexture(new Vector3(-halfSize, -halfSize, +halfSize), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, -halfSize, +halfSize), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, -halfSize, -halfSize), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-halfSize, -halfSize, -halfSize), new Vector2(0, 1)),
                // Left                                                               
                new VertexPositionTexture(new Vector3(-halfSize, +halfSize, -halfSize), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-halfSize, +halfSize, +halfSize), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-halfSize, -halfSize, +halfSize), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-halfSize, -halfSize, -halfSize), new Vector2(0, 1)),
                // Right                                                              
                new VertexPositionTexture(new Vector3(+halfSize, +halfSize, +halfSize), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, +halfSize, -halfSize), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, -halfSize, -halfSize), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(+halfSize, -halfSize, +halfSize), new Vector2(0, 1)),
                // Back                                                               
                new VertexPositionTexture(new Vector3(+halfSize, +halfSize, -halfSize), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-halfSize, +halfSize, -halfSize), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-halfSize, -halfSize, -halfSize), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(+halfSize, -halfSize, -halfSize), new Vector2(0, 1)),
                // Front                                                              
                new VertexPositionTexture(new Vector3(-halfSize, +halfSize, +halfSize), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, +halfSize, +halfSize), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+halfSize, -halfSize, +halfSize), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-halfSize, -halfSize, +halfSize), new Vector2(0, 1))
            };

            return vertices;
        }

        private static ushort[] GetCubeIndices()
        {
            ushort[] indices =
            {
                0, 1, 2, 0, 2, 3,
                4, 5, 6, 4, 6, 7,
                8, 9, 10, 8, 10, 11,
                12, 13, 14, 12, 14, 15,
                16, 17, 18, 16, 18, 19,
                20, 21, 22, 20, 22, 23
            };

            return indices;
        }
    }
}