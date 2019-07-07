using System;
using System.Collections.Generic;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceManager : IResourceContainer
    {
        private readonly ResourceContainerCollection _containerCollection;
        private readonly Dictionary<Type, IResourceCollection> _loaders;

        public ResourceManager()
        {
            _containerCollection = new ResourceContainerCollection(this);
            _loaders = new Dictionary<Type, IResourceCollection>();
            _loaders[typeof(IResourceContainer)] = _containerCollection;
        }

        public IResourceHandler<T> Resolve<T>(ResourceId id)
        {
            IResourceCollection genericCollection;
            if (!_loaders.TryGetValue(typeof(T), out genericCollection)) return _containerCollection.Resolve<T>(id);

            var container = (IResourceCollection<T>) genericCollection;
            return container.Resolve(id);
        }

        public void Register<T>(IResourceCollection<T> collection)
        {
            _loaders.Add(typeof(T), collection);
        }

        public void Register(ResourceLoader<IResourceContainer> loader, params string[] extensions)
        {
            _containerCollection.Register(loader, extensions);
        }

        public void Register<T>(ResourceLoader<T> loader)
        {
            _loaders.Add(typeof(T), new ResourceCollection<T>(this, loader));
        }
    }
}