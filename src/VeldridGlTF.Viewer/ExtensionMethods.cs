using System.Collections.Generic;
using System.Numerics;

namespace VeldridGlTF.Viewer
{
    public static class ExtensionMethods
    {
        public static IEnumerable<float> GetComponents(this Matrix4x4 m)
        {
            yield return m.M11;
            yield return m.M12;
            yield return m.M13;
            yield return m.M14;

            yield return m.M21;
            yield return m.M22;
            yield return m.M23;
            yield return m.M24;

            yield return m.M31;
            yield return m.M32;
            yield return m.M33;
            yield return m.M34;

            yield return m.M41;
            yield return m.M42;
            yield return m.M43;
            yield return m.M44;
        }
    }
}