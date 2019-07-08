using System.Collections.Generic;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ModelRenderCache
    {
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;
        public List<DrawCall> DrawCalls { get; set; }

        public DeviceBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
            set { _vertexBuffer = value; }
        }

        public DeviceBuffer IndexBuffer
        {
            get { return _indexBuffer; }
            set { _indexBuffer = value; }
        }
    }
}