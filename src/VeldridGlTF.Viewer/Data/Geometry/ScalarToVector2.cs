using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class ScalarToVector2 : GeometryStreamConverter<float, Vector2>
    {
        public ScalarToVector2(IReadOnlyList<float> data) : base(data)
        {
        }

        protected override Vector2 Convert(float value)
        {
            return new Vector2(value, 0.0f);
        }
    }
}