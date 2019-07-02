using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class Vec2ElementAccessor : AbstractElementAccessor
    {
        private IList<Vector2> _array;

        public Vec2ElementAccessor(string key, Accessor accessor) : base(key, VertexElementFormat.Float2, accessor)
        {
            _array = accessor.AsVector2Array();
        }
        public override int Size
        {
            get { return 4*2; }
        }
        public override void Write(int index, VertexBufferStream vertexBuffer)
        {
            var vec = _array[index];
            vertexBuffer.Write(vec.X);
            vertexBuffer.Write(vec.Y);
        }

    }
}