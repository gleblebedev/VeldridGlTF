namespace VeldridGlTF.Viewer.Data
{
    public interface IMaterialDescription
    {
        string ShaderName { get; }
        MapParameters Normal { get; }
        MapParameters Emissive { get; }
        MapParameters Occlusion { get; }
        SpecularGlossiness SpecularGlossiness { get; }
        MetallicRoughness MetallicRoughness { get; }
        bool DepthTestEnabled { get; }
        bool DepthWriteEnabled { get; }

        AlphaMode AlphaMode { get; }
        float AlphaCutoff { get; }
        bool Unlit { get; }
    }
}