using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class Vector2ToVector4 : GeometryStreamConverter<Vector2, Vector4>
    {
        public Vector2ToVector4(IReadOnlyList<Vector2> data) : base(data)
        {
        }

        protected override Vector4 Convert(Vector2 value)
        {
            return new Vector4(value.X, value.Y, 0.0f, 0.0f);
        }
    }
}