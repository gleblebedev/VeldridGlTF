using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class FloatElementAccessor: AbstractElementAccessor
    {
        private readonly int _components;

        public FloatElementAccessor(string key, VertexElementFormat format, int components, Accessor accessor) : base(key, format, accessor)
        {
            _components = components;
        }
    }
}