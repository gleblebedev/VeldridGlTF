using System.Numerics;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class Vec4ConstantAccessor : AbstractElementAccessor
    {
        private readonly Vector4 _value;

        public Vec4ConstantAccessor(string key, Vector4 value) : base(key, VertexElementFormat.Float4, null)
        {
            _value = value;
        }
        public override int Size
        {
            get { return 4 * 4; }
        }
        public override void Write(int index, VertexBufferStream vertexBuffer)
        {
            var vec = _value;
            vertexBuffer.Write(vec.X);
            vertexBuffer.Write(vec.Y);
            vertexBuffer.Write(vec.Z);
            vertexBuffer.Write(vec.W);
        }
    }
}