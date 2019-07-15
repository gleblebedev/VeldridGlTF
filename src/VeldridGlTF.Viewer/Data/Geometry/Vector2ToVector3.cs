using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector2ToVector3 : GeometryStreamConverter<Vector2, Vector3>
    {
        public Vector2ToVector3(IReadOnlyList<Vector2> data) : base(data)
        {
        }

        protected override Vector3 Convert(Vector2 value)
        {
            return new Vector3(value.X, value.Y, 0.0f);
        }
    }
}