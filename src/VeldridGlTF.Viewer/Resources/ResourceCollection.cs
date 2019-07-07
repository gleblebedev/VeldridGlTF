using System.Collections.Concurrent;

namespace VeldridGlTF.Viewer.Resources
{
    internal class ResourceCollection : IResourceCollection
    {
        protected ResourceCollection(ResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
        }

        public ResourceManager ResourceManager { get; }
    }

    internal class ResourceCollection<T> : ResourceCollection, IResourceCollection<T>
    {
        private readonly ResourceLoader<T> _loader;

        private readonly ConcurrentDictionary<ResourceId, ResourceHandler<T>> _resources =
            new ConcurrentDictionary<ResourceId, ResourceHandler<T>>();

        public ResourceCollection(ResourceManager resourceManager, ResourceLoader<T> loader) : base(resourceManager)
        {
            _loader = loader;
        }

        public IResourceHandler<T> Resolve(ResourceId id)
        {
            return _resources.GetOrAdd(id, CreateResourceHandler);
        }

        private ResourceHandler<T> CreateResourceHandler(ResourceId id)
        {
            return new ResourceHandler<T>(id, _loader, ResourceManager);
        }
    }
}