using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public struct ScheduledDrawCall
    {
        private DrawCall _drawCall;
        private uint _objectPropertyOffset;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;

        public DrawCall DrawCall
        {
            get { return _drawCall; }
            set { _drawCall = value; }
        }

        public uint ObjectPropertyOffset
        {
            get { return _objectPropertyOffset; }
            set { _objectPropertyOffset = value; }
        }

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