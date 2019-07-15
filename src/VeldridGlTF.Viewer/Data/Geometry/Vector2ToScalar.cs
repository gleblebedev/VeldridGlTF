using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector2ToScalar : GeometryStreamConverter<Vector2, float>
    {
        public Vector2ToScalar(IReadOnlyList<Vector2> data) : base(data)
        {
        }

        protected override float Convert(Vector2 value)
        {
            return value.X;
        }
    }
}