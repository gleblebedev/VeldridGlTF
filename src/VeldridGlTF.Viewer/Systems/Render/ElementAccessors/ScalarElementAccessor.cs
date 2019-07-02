using System.Collections.Generic;
using System.IO;
using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class ScalarElementAccessor: AbstractElementAccessor
    {
        private IList<float> _array;

        public ScalarElementAccessor(string key, Accessor accessor) : base(key, VertexElementFormat.Float1, accessor)
        {
            _array = accessor.AsScalarArray();
        }

        public override int Size
        {
            get { return 4; }
        }

        public override void Write(int index, VertexBufferStream vertexBuffer)
        {
            vertexBuffer.Write(_array[index]);
        }
    }
}