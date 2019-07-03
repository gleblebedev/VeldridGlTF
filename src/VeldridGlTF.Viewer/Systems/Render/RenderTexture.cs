using System.IO;
using System.Linq;
using Veldrid;
using Veldrid.ImageSharp;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;
using Texture = SharpGLTF.Schema2.Texture;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderTexture : ITexture
    {
        private readonly ResourceId _id;
        private TextureView _textureView;
        private ImageSharpTexture _image;
        private Veldrid.Texture _texture;

        public RenderTexture(ResourceId id, Texture texture)
        {
            _id = id;
            _image = new ImageSharpTexture(new MemoryStream(texture.PrimaryImage.GetImageContent().ToArray()));
        }

        public TextureView TextureView
        {
            get { return _textureView; }
            set { _textureView = value; }
        }

        public void EnsureResources(VeldridRenderSystem veldridRenderSystem)
        {
            if (_textureView != null)
            {
                return;
            }

            _texture = _image.CreateDeviceTexture(veldridRenderSystem.GraphicsDevice, veldridRenderSystem.ResourceFactory);
            _textureView = veldridRenderSystem.ResourceFactory.CreateTextureView(_texture);
        }
    }
}