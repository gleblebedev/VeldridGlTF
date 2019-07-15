using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector3ToVector2 : GeometryStreamConverter<Vector3, Vector2>
    {
        public Vector3ToVector2(IReadOnlyList<Vector3> data) : base(data)
        {
        }

        protected override Vector2 Convert(Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }
    }
}