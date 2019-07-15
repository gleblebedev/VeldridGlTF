using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector3ToVector4 : GeometryStreamConverter<Vector3, Vector4>
    {
        public Vector3ToVector4(IReadOnlyList<Vector3> data) : base(data)
        {
        }

        protected override Vector4 Convert(Vector3 value)
        {
            return new Vector4(value.X, value.Y, value.Z, 0.0f);
        }
    }
}