using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class Vec4ElementAccessor : AbstractElementAccessor
    {
        private IList<Vector4> _array;

        public Vec4ElementAccessor(string key, Accessor accessor) : base(key, VertexElementFormat.Float4, accessor)
        {
            _array = accessor.AsVector4Array();
        }
        public override int Size
        {
            get { return 4 * 4; }
        }
        public override void Write(int index, VertexBufferStream vertexBuffer)
        {
            var vec = _array[index];
            vertexBuffer.Write(vec.X);
            vertexBuffer.Write(vec.Y);
            vertexBuffer.Write(vec.Z);
            vertexBuffer.Write(vec.W);
        }
    }
}