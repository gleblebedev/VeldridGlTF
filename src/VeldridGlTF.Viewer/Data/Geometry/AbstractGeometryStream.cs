using System.Collections.Generic;
using System.Numerics;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public abstract class AbstractGeometryStream
    {
        protected AbstractGeometryStream(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public abstract void Write(int index, VertexBufferStream vbStream);

        public static IGeometryStream Create(string key, IReadOnlyList<float> data)
        {
            return new ScalarGeometryStream(key, data);
        }

        public static IGeometryStream Create(string key, IReadOnlyList<Vector2> data)
        {
            return new Vector2GeometryStream(key, data);
        }

        public static IGeometryStream Create(string key, IReadOnlyList<Vector3> data)
        {
            return new Vector3GeometryStream(key, data);
        }

        public static IGeometryStream Create(string key, IReadOnlyList<Vector4> data)
        {
            return new Vector4GeometryStream(key, data);
        }
    }
}