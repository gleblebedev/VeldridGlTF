using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public struct ScheduledDrawCall
    {
        public DrawCall DrawCall { get; set; }

        public uint ObjectPropertyOffset { get; set; }

        public uint JointMatrices { get; set; }
        public uint JointNormalMatrices { get; set; }

        public DeviceBuffer VertexBuffer { get; set; }

        public DeviceBuffer IndexBuffer { get; set; }
    }
}