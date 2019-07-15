using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector4ToVector3 : GeometryStreamConverter<Vector4, Vector3>
    {
        public Vector4ToVector3(IReadOnlyList<Vector4> data) : base(data)
        {
        }

        protected override Vector3 Convert(Vector4 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }
    }
}