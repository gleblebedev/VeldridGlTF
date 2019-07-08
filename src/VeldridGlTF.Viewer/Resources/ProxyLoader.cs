using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    internal class ProxyLoader<T> : IResourceLoader<T>
    {
        private readonly IResourceHandler<IResourceContainer> _containerHandler;

        public ProxyLoader(IResourceHandler<IResourceContainer> containerHandler)
        {
            _containerHandler = containerHandler;
        }

        public async Task<T> LoadAsync(ResourceContext context)
        {
            var container = await context.ResolveDependencyAsync(_containerHandler);
            return await context.ResolveDependencyAsync(container.Resolve<T>(context.Id));
        }
    }
}