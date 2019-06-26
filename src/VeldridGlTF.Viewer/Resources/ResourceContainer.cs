﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceContainer<T> : IResourceContainer
    {
        private readonly IResourceLoader<T> _loader;
        private readonly ResourceManager _manager;
        private object _gate = new object();

        private readonly ConcurrentDictionary<ResourceId, IResourceHandler<T>> _handlers =
            new ConcurrentDictionary<ResourceId, IResourceHandler<T>>();

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
            return _handlers.GetOrAdd(id, _ => new ResourceHandler<T>(_manager, _, _loader));
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