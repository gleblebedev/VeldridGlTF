using System.Collections.Generic;
using Veldrid;
using Veldrid.Utilities;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class RenderMesh : AbstractResource, IMesh
    {
        public readonly List<RenderPrimitive> _primitives;
        public DeviceBuffer _indexBuffer;
        public DeviceBuffer _vertexBuffer;
        private BoundingBox _boundingBox;

        public RenderMesh(ResourceId id, DeviceBuffer vertexBuffer, DeviceBuffer indexBuffer, BoundingBox boundingBox,
            List<RenderPrimitive> primitives) : base(id)
        {
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
            _primitives = primitives;
            _boundingBox = boundingBox;
        }

        public List<RenderPrimitive> Primitives => _primitives;

        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
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