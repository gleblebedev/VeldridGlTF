using System;
using System.Threading;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceHandler<T> : IResourceHandler<T>
    {
        private readonly ResourceId _id;
        private readonly Lazy<Task<T>> _taskFactory;

        public ResourceHandler(ResourceManager manager, ResourceId id, IResourceLoader<T> loader) : this(id,
            () => loader.LoadAsync(manager, id))
        {
        }

        public ResourceHandler(ResourceId id, Func<Task<T>> factory)
        {
            _id = id;
            _taskFactory = new Lazy<Task<T>>(factory, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public TaskStatus Status => _taskFactory.Value.Status;

        public Task<T> GetAsync()
        {
            return _taskFactory.Value;
        }
    }
}