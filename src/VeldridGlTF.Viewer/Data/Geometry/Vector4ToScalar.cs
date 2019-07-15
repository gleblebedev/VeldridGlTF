using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector4ToScalar : GeometryStreamConverter<Vector4, float>
    {
        public Vector4ToScalar(IReadOnlyList<Vector4> data) : base(data)
        {
        }

        protected override float Convert(Vector4 value)
        {
            return value.X;
        }
    }
}