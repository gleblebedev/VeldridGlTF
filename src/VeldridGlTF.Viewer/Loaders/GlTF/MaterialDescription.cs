using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class MaterialDescription : AbstractResource, IMaterialDescription
    {
        public MaterialDescription(ResourceId id) : base(id)
        {
        }

        public string ShaderName { get; set; } = "Default";
        public MapParameters Normal { get; set; }
        public MapParameters Emissive { get; set; }
        public MapParameters Occlusion { get; set; }
        public SpecularGlossiness SpecularGlossiness { get; set; }
        public MetallicRoughness MetallicRoughness { get; set; }

        public float AlphaCutoff { get; set; } = 1.0f;

        public bool DepthTestEnabled { get; set; } = true;

        public bool DepthWriteEnabled { get; set; } = true;

        public AlphaMode AlphaMode { get; set; }

        public bool Unlit { get; set; }
    }
}