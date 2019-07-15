using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public sealed class ScalarToVector3 : GeometryStreamConverter<float, Vector3>
    {
        public ScalarToVector3(IReadOnlyList<float> data) : base(data)
        {
        }

        protected override Vector3 Convert(float value)
        {
            return new Vector3(value, 0.0f, 0.0f);
        }
    }
}