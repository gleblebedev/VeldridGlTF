using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class ScalarToVector4 : GeometryStreamConverter<float, Vector4>
    {
        public ScalarToVector4(IReadOnlyList<float> data) : base(data)
        {
        }

        protected override Vector4 Convert(float value)
        {
            return new Vector4(value, 0.0f, 0.0f, 0.0f);
        }
    }
}