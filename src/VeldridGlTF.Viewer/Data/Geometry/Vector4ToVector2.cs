using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector4ToVector2 : GeometryStreamConverter<Vector4, Vector2>
    {
        public Vector4ToVector2(IReadOnlyList<Vector4> data) : base(data)
        {
        }

        protected override Vector2 Convert(Vector4 value)
        {
            return new Vector2(value.X, value.Y);
        }
    }
}