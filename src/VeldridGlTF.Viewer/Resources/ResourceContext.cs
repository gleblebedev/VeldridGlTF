using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceContext : IDisposable
    {
        private readonly List<ResourceHandler> _dependencies;
        private readonly ResourceManager _manager;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;

        public ResourceContext(ResourceId id, ResourceManager manager)
        {
            Id = id;
            _manager = manager;
            _dependencies = new List<ResourceHandler>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken Token => _cancellationTokenSource.Token;

        public ResourceId Id { get; }

        public virtual void Dispose()
        {
            _isDisposed = true;
            _cancellationTokenSource.Cancel();
            foreach (var handler in _dependencies) handler.ResourceChanged -= InvalidateContext;
        }

        public event EventHandler ResourceInvalid;

        protected void AddDependency<Dependency>(IResourceHandler<Dependency> handler)
        {
            if (_isDisposed) throw new ObjectDisposedException("ResourceContext is disposed");
            handler.ResourceChanged += InvalidateContext;
        }

        protected void InvalidateContext(object sender, EventArgs e)
        {
            if (_isDisposed) return;
            ResourceInvalid?.Invoke(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public async Task<Dependency> ResolveDependencyAsync<Dependency>(ResourceId resourceId)
        {
            if (_isDisposed) throw new ObjectDisposedException("ResourceContext is disposed");
            var handler = _manager.Resolve<Dependency>(resourceId);
            AddDependency(handler);
            return await handler.GetAsync();
        }

        public async Task<Dependency> ResolveDependencyAsync<Dependency>(IResourceHandler<Dependency> handler)
        {
            if (_isDisposed) throw new ObjectDisposedException("ResourceContext is disposed");
            AddDependency(handler);
            return await handler.GetAsync();
        }

        public IResourceHandler<T> Resolve<T>(ResourceId resourceId)
        {
            return _manager.Resolve<T>(resourceId);
        }
    }

    public class ResourceContext<T> : ResourceContext
    {
        private readonly ResourceLoader<T> _loader;

        public ResourceContext(ResourceId id, ResourceManager manager, ResourceLoader<T> loader) : base(id, manager)
        {
            _loader = loader;
        }

        public Task<T> Task { get; private set; }

        public void Start()
        {
            try
            {
                Task = _loader.LoadAsync(this);
            }
            catch (Exception ex)
            {
                Task = System.Threading.Tasks.Task.FromException<T>(ex);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}