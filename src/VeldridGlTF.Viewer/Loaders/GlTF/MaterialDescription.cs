using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{

    public class MaterialDescription : AbstractResource, IMaterialDescription
    {
        private bool _depthTestEnabled = true;
        private bool _depthWriteEnabled = true;
        private AlphaMode _alphaMode;
        private bool _unlit;

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

        public bool DepthTestEnabled
        {
            get { return _depthTestEnabled; }
            set { _depthTestEnabled = value; }
        }

        public bool DepthWriteEnabled
        {
            get { return _depthWriteEnabled; }
            set { _depthWriteEnabled = value; }
        }

        public AlphaMode AlphaMode
        {
            get { return _alphaMode; }
            set { _alphaMode = value; }
        }

        public bool Unlit
        {
            get { return _unlit; }
            set { _unlit = value; }
        }
    }

}