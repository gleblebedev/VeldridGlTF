using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public class Vector3GeometryStream : AbstractGeometryStream, IGeometryStream
    {
        private readonly IReadOnlyList<Vector3> _data;

        public Vector3GeometryStream(string key, IReadOnlyList<Vector3> data) : base(key)
        {
            _data = data;
        }

        public GeometryStreamFormat Format => GeometryStreamFormat.Float3;

        public override void Write(int index, VertexBufferStream vbStream)
        {
            var vec = _data[index];
            vbStream.Write(vec.X);
            vbStream.Write(vec.Y);
            vbStream.Write(vec.Z);
        }

        public IReadOnlyList<float> AsScalar()
        {
            return new Vector3ToScalar(_data);
        }

        public IReadOnlyList<Vector2> AsVector2()
        {
            return new Vector3ToVector2(_data);
        }

        public IReadOnlyList<Vector3> AsVector3()
        {
            return _data;
        }

        public IReadOnlyList<Vector4> AsVector4()
        {
            return new Vector3ToVector4(_data);
        }

        public static IReadOnlyList<Vector3> AsReadOnlyList(IList<Vector3> list)
        {
            var res = list as IReadOnlyList<Vector3>;
            if (res != null)
                return res;
            return list.ToList();
        }
    }
}