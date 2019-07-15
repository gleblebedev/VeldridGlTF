using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public class Vector4GeometryStream : AbstractGeometryStream, IGeometryStream
    {
        private readonly IReadOnlyList<Vector4> _data;

        public Vector4GeometryStream(string key, IReadOnlyList<Vector4> data) : base(key)
        {
            _data = data;
        }

        public GeometryStreamFormat Format => GeometryStreamFormat.Float4;

        public override void Write(int index, VertexBufferStream vbStream)
        {
            var vec = _data[index];
            vbStream.Write(vec.X);
            vbStream.Write(vec.Y);
            vbStream.Write(vec.Z);
            vbStream.Write(vec.W);
        }

        public IReadOnlyList<float> AsScalar()
        {
            return new Vector4ToScalar(_data);
        }

        public IReadOnlyList<Vector2> AsVector2()
        {
            return new Vector4ToVector2(_data);
        }

        public IReadOnlyList<Vector3> AsVector3()
        {
            return new Vector4ToVector3(_data);
        }

        public IReadOnlyList<Vector4> AsVector4()
        {
            return _data;
        }

        public static IReadOnlyList<Vector4> AsReadOnlyList(IList<Vector4> list)
        {
            var res = list as IReadOnlyList<Vector4>;
            if (res != null)
                return res;
            return list.ToList();
        }
    }
}