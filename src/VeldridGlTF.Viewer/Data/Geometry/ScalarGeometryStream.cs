using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public class ScalarGeometryStream : AbstractGeometryStream, IGeometryStream
    {
        private readonly IReadOnlyList<float> _data;
        private GeometryStreamFormat _format;

        public ScalarGeometryStream(string key, IReadOnlyList<float> data) : base(key)
        {
            _data = data;
        }

        public GeometryStreamFormat Format => GeometryStreamFormat.Float1;

        public override void Write(int index, VertexBufferStream vbStream)
        {
            vbStream.Write(_data[index]);
        }

        public IReadOnlyList<float> AsScalar()
        {
            return _data;
        }

        public IReadOnlyList<Vector2> AsVector2()
        {
            return new ScalarToVector2(_data);
        }


        public IReadOnlyList<Vector3> AsVector3()
        {
            return new ScalarToVector3(_data);
        }

        public IReadOnlyList<Vector4> AsVector4()
        {
            return new ScalarToVector4(_data);
        }

        public static IReadOnlyList<float> AsReadOnlyList(IList<float> list)
        {
            var res = list as IReadOnlyList<float>;
            if (res != null)
                return res;
            return list.ToList();
        }
    }
}