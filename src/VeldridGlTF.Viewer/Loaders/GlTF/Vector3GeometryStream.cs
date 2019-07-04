using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class FloatGeometryStream : GeometryStream
    {
        private readonly IList<float> _data;

        public FloatGeometryStream(string key, Accessor accessor) : base(key, GeometryStreamFormat.Float1)
        {
            _data = accessor.AsScalarArray();
        }

        public override void Write(int index, VertexBufferStream vbStream)
        {
            vbStream.Write(_data[index]);
        }
    }

    public class Vector2GeometryStream : GeometryStream
    {
        private readonly IList<Vector2> _data;

        public Vector2GeometryStream(string key, Accessor accessor) : base(key, GeometryStreamFormat.Float2)
        {
            _data = accessor.AsVector2Array();
        }

        public override void Write(int index, VertexBufferStream vbStream)
        {
            var vec = _data[index];
            vbStream.Write(vec.X);
            vbStream.Write(vec.Y);
        }
    }

    public class Vector3GeometryStream : GeometryStream
    {
        private readonly IList<Vector3> _data;

        public Vector3GeometryStream(string key, Accessor accessor) : base(key, GeometryStreamFormat.Float3)
        {
            _data = accessor.AsVector3Array();
        }

        public override void Write(int index, VertexBufferStream vbStream)
        {
            var vec = _data[index];
            vbStream.Write(vec.X);
            vbStream.Write(vec.Y);
            vbStream.Write(vec.Z);
        }
    }

    public class Vector4GeometryStream : GeometryStream
    {
        private readonly IList<Vector4> _data;

        public Vector4GeometryStream(string key, Accessor accessor) : base(key, GeometryStreamFormat.Float4)
        {
            _data = accessor.AsVector4Array();
        }

        public override void Write(int index, VertexBufferStream vbStream)
        {
            var vec = _data[index];
            vbStream.Write(vec.X);
            vbStream.Write(vec.Y);
            vbStream.Write(vec.Z);
            vbStream.Write(vec.W);
        }
    }
}