using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class Vec3ElementAccessor : AbstractElementAccessor
    {
        private IList<Vector3> _array;

        public Vec3ElementAccessor(string key, Accessor accessor) : base(key, VertexElementFormat.Float3, accessor)
        {
            _array = accessor.AsVector3Array();
        }
        public override int Size
        {
            get { return 4 * 3; }
        }
        public override void Write(int index, VertexBufferStream vertexBuffer)
        {
            var vec = _array[index];
            vertexBuffer.Write(vec.X);
            vertexBuffer.Write(vec.Y);
            vertexBuffer.Write(vec.Z);
        }
    }
}