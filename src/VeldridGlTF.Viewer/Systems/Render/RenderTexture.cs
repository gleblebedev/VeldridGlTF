using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderTexture : ITexture
    {
        private readonly ResourceId _id;

        public RenderTexture(ResourceId id, Texture texture)
        {
            _id = id;
        }
    }
}