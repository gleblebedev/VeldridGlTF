using System.Numerics;

namespace VeldridGlTF.Viewer.Data
{
    public class SpecularGlossiness : PBRParameters
    {
        public Vector3 SpecularFactor = Vector3.Zero;
        public Vector4 DiffuseFactor = Vector4.One;
        public float GlossinessFactor = 0.5f;
        public MapParameters Diffuse { get; set; }
        public MapParameters SpecularGlossinessMap { get; set; }
    }
}