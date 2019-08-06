namespace VeldridGlTF.Viewer.Data
{
    public class MetallicRoughness : PBRParameters
    {
        public MapParameters BaseColor { get; set; }

        public MapParameters MetallicRoughnessMap { get; set; }
    }
}