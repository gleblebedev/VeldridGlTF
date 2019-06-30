using System;
using System.Collections.Generic;
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
        private readonly VertexPositionTexture[] _vertices;

        public DeviceBuffer _indexBuffer;
        public DeviceBuffer _vertexBuffer;
        public List<IndexRange> Primitives = new List<IndexRange>();
        private BoundingBox _bbox;

        public RenderMesh(Mesh mesh)
        {
            var vertices = new List<VertexPositionTexture>();
            var indices = new List<ushort>();
            
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
                var normals = GetMemoryAccessor(meshPrimitive.GetVertexAccessor("NORMAL"))?.AsVector3Array();
                var texCoords0 = GetMemoryAccessor(meshPrimitive.GetVertexAccessor("TEXCOORD_0"))?.AsVector2Array();
                var map = new Dictionary<int, ushort>();
                var range = new IndexRange();
                range.Start = (uint) indices.Count;
                foreach (var face in meshPrimitive.GetTriangleIndices())
                {
                    indices.Add(CopyVertex(face.Item1, map, vertices, positions, normals, texCoords0));
                    indices.Add(CopyVertex(face.Item2, map, vertices, positions, normals, texCoords0));
                    indices.Add(CopyVertex(face.Item3, map, vertices, positions, normals, texCoords0));
                }

                range.Length = (uint) (indices.Count - range.Start);
                Primitives.Add(range);
            }

            _bbox = bbox;
            _vertices = vertices.ToArray();
            _indices = indices.ToArray();
        }

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
            vertexPositionTexture.Pos = pos;
            if (texCoords0 != null)
            {
                var uv = texCoords0.Value[originalIndex];
                vertexPositionTexture.UV = uv;
            }

            if (normals != null)
            {
                var n = normals.Value[originalIndex];
                vertexPositionTexture.Normal = n;
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