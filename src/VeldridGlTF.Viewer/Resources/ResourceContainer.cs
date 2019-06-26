using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceContainer<T>: IResourceContainer
    {
        private readonly ResourceManager _manager;
        private readonly IResourceLoader<T> _loader;
        ConcurrentDictionary<ResourceId, IResourceHandler<T>> _handlers = new ConcurrentDictionary<ResourceId, IResourceHandler<T>>();
        object _gate = new object();

        public ResourceContainer(ResourceManager manager, IResourceLoader<T> loader)
        {
            _manager = manager;
            _loader = loader;
        }

        IResourceHandler IResourceContainer.Resolve(ResourceId id)
        {
            return Resolve(id);
        }

        public IResourceHandler<T> Resolve(ResourceId id)
        {
            return _handlers.GetOrAdd(id, _ => new ResourceHandler<T>(_manager,_, _loader));
        }

        public IResourceHandler<T> ResolveOrAdd(ResourceId id, Func<Task<T>> factory)
        {
            return _handlers.GetOrAdd(id, _ => new ResourceHandler<T>(_, factory));
        }

        public IResourceHandler<T> ResolveOrAdd(ResourceId id, IResourceHandler<T> handler)
        {
            return _handlers.GetOrAdd(id, handler);
        }
    }
}