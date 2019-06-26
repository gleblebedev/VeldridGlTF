using System;
using System.Threading.Tasks;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders
{
    public class GlTFLoader : IResourceLoader<GlTFContainer>
    {
        public async Task<GlTFContainer> LoadAsync(ResourceManager manager, ResourceId id)
        {
            if (id.Id != null)
                throw new ArgumentException("Resource id value should be null", nameof(id));
            return new GlTFContainer(manager, id.Container,
                ModelRoot.Read(GetType().Assembly.GetManifestResourceStream(id.Container), new ReadSettings()));
        }
    }
}