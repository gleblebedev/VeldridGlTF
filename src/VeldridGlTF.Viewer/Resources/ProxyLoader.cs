using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    internal class ProxyLoader<T> : ResourceLoader<T>
    {
        private readonly IResourceHandler<IResourceContainer> _containerHandler;

        public ProxyLoader(IResourceHandler<IResourceContainer> containerHandler)
        {
            _containerHandler = containerHandler;
        }

        public override async Task<T> LoadAsync(ResourceContext context)
        {
            var container = await context.ResolveDependencyAsync(_containerHandler);
            return await context.ResolveDependencyAsync(container.Resolve<T>(context.Id));
        }
    }
}