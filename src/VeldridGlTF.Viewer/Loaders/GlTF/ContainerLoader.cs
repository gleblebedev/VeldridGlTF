using System;
using System.Threading.Tasks;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class ContainerLoader : IResourceLoader<GlTFContainer>
    {
        public Task<GlTFContainer> LoadAsync(ResourceManager manager, ResourceId id)
        {
            if (id.Id != null)
                throw new ArgumentException("Resource id value should be null", nameof(id));
            return Task.Run(() => new GlTFContainer(manager, id.Container,
                ModelRoot.Read(GetType().Assembly.GetManifestResourceStream(id.Container), new ReadSettings())));
        }
    }
}