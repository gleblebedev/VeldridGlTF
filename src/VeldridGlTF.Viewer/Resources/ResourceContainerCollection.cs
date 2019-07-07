using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceContainerCollection : IResourceCollection<IResourceContainer>
    {
        private readonly ConcurrentDictionary<string, ResourceHandler<IResourceContainer>> _containers =
            new ConcurrentDictionary<string, ResourceHandler<IResourceContainer>>();

        private readonly Dictionary<string, ResourceLoader<IResourceContainer>> _loaderByExtension =
            new Dictionary<string, ResourceLoader<IResourceContainer>>();

        private readonly ResourceManager _manager;

        private readonly ConcurrentDictionary<ResourceIdAndType, IResourceHandler> _resources =
            new ConcurrentDictionary<ResourceIdAndType, IResourceHandler>();

        public ResourceContainerCollection(ResourceManager manager)
        {
            _manager = manager;
        }

        public IResourceHandler<IResourceContainer> Resolve(ResourceId id)
        {
            return _containers.GetOrAdd(id.Path, _ => CreateConainer(_));
        }

        public IResourceHandler<T> Resolve<T>(ResourceId id)
        {
            var key = new ResourceIdAndType {Id = id, Type = typeof(T)};
            return (IResourceHandler<T>) _resources.GetOrAdd(key, _ => CreateHandler<T>(id));
        }

        private IResourceHandler<T> CreateHandler<T>(ResourceId id)
        {
            var containerHandler = Resolve(id);
            if (containerHandler == null)
            {
                var manualResourceHandler = new ManualResourceHandler<T>(id);
                manualResourceHandler.SetException(new ResourceException(id,
                    "Container file extention " + id + " isn't supported."));
                return manualResourceHandler;
            }

            return new ResourceHandler<T>(id, new ProxyLoader<T>(containerHandler), _manager);
        }

        private ResourceHandler<IResourceContainer> CreateConainer(string url)
        {
            ResourceLoader<IResourceContainer> loader;
            if (!_loaderByExtension.TryGetValue(Path.GetExtension(url).ToLower(), out loader)) return null;

            return new ResourceHandler<IResourceContainer>(new ResourceId(url), loader, _manager);
        }

        public void Register(ResourceLoader<IResourceContainer> loader, string[] extensions)
        {
            if (extensions == null || extensions.Length == 0)
                throw new ArgumentException("Empty extensions collection", nameof(extensions));

            foreach (var extension in extensions) _loaderByExtension.Add(extension.ToLower(), loader);
        }

        private struct ResourceIdAndType
        {
            public ResourceId Id;
            public Type Type;
        }
    }
}