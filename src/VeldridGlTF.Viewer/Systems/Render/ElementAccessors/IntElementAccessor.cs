using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class IntElementAccessor : AbstractElementAccessor
    {
        private readonly int _components;

        public IntElementAccessor(string key, VertexElementFormat format, int components, Accessor accessor) : base(key, format, accessor)
        {
            _components = components;
        }
    }
}