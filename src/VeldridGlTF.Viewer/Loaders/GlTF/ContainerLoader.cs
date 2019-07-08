using System;
using System.Threading.Tasks;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class ContainerLoader : IResourceLoader<IResourceContainer>
    {
        private ReadSettings _readSettings;

        public ContainerLoader()
        {
            _readSettings = new ReadSettings();
        }

        public async Task<IResourceContainer> LoadAsync(ResourceContext context)
        {
            if (context.Id.Id != null)
                throw new ArgumentException("Resource id value should be null", nameof(context));

            var container = new GlTFContainer();
            try
            {
                await container.ParseFile(context);
            }
            catch (Exception ex)
            {
                //container.Dispose();
                throw new ResourceException(context.Id, ex);
            }

            return container;
        }
    }
}