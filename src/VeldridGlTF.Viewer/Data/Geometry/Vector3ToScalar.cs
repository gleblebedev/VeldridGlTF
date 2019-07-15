using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector3ToScalar : GeometryStreamConverter<Vector3, float>
    {
        public Vector3ToScalar(IReadOnlyList<Vector3> data) : base(data)
        {
        }

        protected override float Convert(Vector3 value)
        {
            return value.X;
        }
    }
}