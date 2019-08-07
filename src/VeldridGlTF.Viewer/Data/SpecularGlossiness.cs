using System.Numerics;

namespace VeldridGlTF.Viewer.Data
{
    public class SpecularGlossiness : PBRParameters
    {
        public MapParameters Diffuse { get; set; }
        public MapParameters SpecularGlossinessMap { get; set; }
    }
}