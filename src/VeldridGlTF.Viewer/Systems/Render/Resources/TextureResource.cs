using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class TextureResource : AbstractResource, ITexture
    {
        public TextureResource(ResourceId id, Texture deviceTexture, TextureView view) : base(id)
        {
            DeviceTexture = deviceTexture;
            View = view;
        }

        public Texture DeviceTexture { get; }

        public TextureView View { get; }
    }
}