using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public class Vector2GeometryStream : AbstractGeometryStream, IGeometryStream
    {
        private readonly IReadOnlyList<Vector2> _data;

        public Vector2GeometryStream(string key, IReadOnlyList<Vector2> data) : base(key)
        {
            _data = data;
        }

        public GeometryStreamFormat Format => GeometryStreamFormat.Float2;

        public override void Write(int index, VertexBufferStream vbStream)
        {
            var vec = _data[index];
            vbStream.Write(vec.X);
            vbStream.Write(vec.Y);
        }

        public IReadOnlyList<float> AsScalar()
        {
            return new Vector2ToScalar(_data);
        }

        public IReadOnlyList<Vector2> AsVector2()
        {
            return _data;
        }

        public IReadOnlyList<Vector3> AsVector3()
        {
            return new Vector2ToVector3(_data);
        }

        public IReadOnlyList<Vector4> AsVector4()
        {
            return new Vector2ToVector4(_data);
        }

        public static IReadOnlyList<Vector2> AsReadOnlyList(IList<Vector2> list)
        {
            var res = list as IReadOnlyList<Vector2>;
            if (res != null)
                return res;
            return list.ToList();
        }
    }
}