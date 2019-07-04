using System.IO;
using System.Linq;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class EmbeddedImage : AbstractResource, IImage
    {
        private readonly Texture _texture;

        public EmbeddedImage(ResourceId id, Texture texture) : base(id)
        {
            _texture = texture;
        }

        public Stream Open()
        {
            return new MemoryStream(_texture.PrimaryImage.GetImageContent().ToArray());
        }
    }
}