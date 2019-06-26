using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceManager
    {
        private readonly Dictionary<Type, IResourceContainer> _loaders = new Dictionary<Type, IResourceContainer>();

        public void Register<T>(IResourceLoader<T> loader)
        {
            _loaders.Add(typeof(T), new ResourceContainer<T>(this, loader));
        }

        public IResourceHandler Resolve(Type resourceType, ResourceId id)
        {
            IResourceContainer genericContainer;
            if (!_loaders.TryGetValue(resourceType, out genericContainer))
                return null;

            return genericContainer.Resolve(id);
        }

        public IResourceHandler<T> Resolve<T>(ResourceId id)
        {
            IResourceContainer genericContainer;
            if (!_loaders.TryGetValue(typeof(T), out genericContainer))
                return null;

            var container = (ResourceContainer<T>) genericContainer;
            return container.Resolve(id);
        }

        public IResourceHandler<T> ResolveOrAdd<T>(ResourceId id, Func<Task<T>> factory)
        {
            IResourceContainer genericContainer;
            if (!_loaders.TryGetValue(typeof(T), out genericContainer))
                return null;

            var container = (ResourceContainer<T>) genericContainer;
            return container.ResolveOrAdd(id, factory);
        }

        public IResourceHandler<T> ResolveOrAdd<T>(ResourceId id, IResourceHandler<T> handler)
        {
            IResourceContainer genericContainer;
            if (!_loaders.TryGetValue(typeof(T), out genericContainer))
                return null;

            var container = (ResourceContainer<T>) genericContainer;
            return container.ResolveOrAdd(id, handler);
        }
    }
}