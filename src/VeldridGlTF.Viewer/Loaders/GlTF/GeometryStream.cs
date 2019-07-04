using System;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public abstract class GeometryStream : IGeometryStream
    {
        protected GeometryStream(string key, GeometryStreamFormat format)
        {
            Key = key;
            Format = format;
        }

        public string Key { get; }

        public GeometryStreamFormat Format { get; }

        public abstract void Write(int index, VertexBufferStream vbStream);

        public static IGeometryStream Create(string key, Accessor accessor)
        {
            switch (accessor.Dimensions)
            {
                case DimensionType.SCALAR:
                    return new FloatGeometryStream(key, accessor);
                case DimensionType.VEC2:
                    return new Vector2GeometryStream(key, accessor);
                case DimensionType.VEC3:
                    return new Vector3GeometryStream(key, accessor);
                case DimensionType.VEC4:
                    return new Vector4GeometryStream(key, accessor);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}