using System;
using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data.Geometry;
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
        public abstract IReadOnlyList<Vector4> AsVector4();
        public abstract IReadOnlyList<Vector3> AsVector3();
        public abstract IReadOnlyList<Vector2> AsVector2();
        public abstract IReadOnlyList<float> AsScalar();

        public static IGeometryStream Create(string key, Accessor accessor)
        {
            switch (accessor.Dimensions)
            {
                case DimensionType.SCALAR:
                    return new ScalarGeometryStream(key, ScalarGeometryStream.AsReadOnlyList(accessor.AsScalarArray()));
                case DimensionType.VEC2:
                    return new Vector2GeometryStream(key,
                        Vector2GeometryStream.AsReadOnlyList(accessor.AsVector2Array()));
                case DimensionType.VEC3:
                    return new Vector3GeometryStream(key,
                        Vector3GeometryStream.AsReadOnlyList(accessor.AsVector3Array()));
                case DimensionType.VEC4:
                    return new Vector4GeometryStream(key,
                        Vector4GeometryStream.AsReadOnlyList(accessor.AsVector4Array()));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}