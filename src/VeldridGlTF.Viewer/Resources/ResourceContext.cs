using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public static class EmptyResult<T>
    {
        public static readonly Task<T> Task = System.Threading.Tasks.Task.FromResult(default(T));
    }

    public class ResourceContext : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<ResourceHandler> _dependencies;
        private readonly IResourceContainer _resourceContainer;
        private bool _isDisposed;

        public ResourceContext(ResourceId id, IResourceContainer resourceContainer)
        {
            Id = id;
            _resourceContainer = resourceContainer;
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
            if (handler == null)
                return;
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

        public Task<Dependency> ResolveDependencyAsync<Dependency>(ResourceId resourceId)
        {
            if (_isDisposed) throw new ObjectDisposedException("ResourceContext is disposed");
            var handler = _resourceContainer.Resolve<Dependency>(resourceId);
            AddDependency(handler);
            return GetTaskFromHandler(handler);
        }

        private async Task<Dependency> GetTaskFromHandler<Dependency>(IResourceHandler<Dependency> handler)
        {
            var task = handler.GetAsync();
            //if (task.IsCompleted || task.IsFaulted)
            //    return task.Result;
            return await task;//.ConfigureAwait(false);
        }

        public Task<Dependency> ResolveDependencyAsync<Dependency>(IResourceHandler<Dependency> handler)
        {
            if (_isDisposed) throw new ObjectDisposedException("ResourceContext is disposed");
            if (handler == null) return EmptyResult<Dependency>.Task;

            AddDependency(handler);
            return GetTaskFromHandler(handler);
        }

        public IResourceHandler<T> Resolve<T>(ResourceId resourceId)
        {
            return _resourceContainer.Resolve<T>(resourceId);
        }
    }

    public class ResourceContext<T> : ResourceContext
    {
        private readonly IResourceLoader<T> _loader;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task<T> _task;

        public ResourceContext(ResourceId id, IResourceContainer resourceContainer, IResourceLoader<T> loader) : base(
            id, resourceContainer)
        {
            _loader = loader;
        }

        public Task<T> Task
        {
            get
            {
                if (_task == null) throw new Exception("ResourceContext is not started yet.");
                return _task;
            }
            private set => _task = value;
        }

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public void Start()
        {
            try
            {
                Task = System.Threading.Tasks.Task.Run(() => _loader.LoadAsync(this));
            }
            catch (Exception ex)
            {
                Task = System.Threading.Tasks.Task.FromException<T>(ex);
            }
        }

        public override void Dispose()
        {
            if (Task.IsCompleted)
                DisposeTaskResult(Task);
            else
                Task.ContinueWith(DisposeTaskResult);
            _cancellationTokenSource.Cancel();
            base.Dispose();
        }

        private void DisposeTaskResult(Task<T> task)
        {
            if (task.IsCompleted)
            {
                var disposable = task.Result as IDisposable;
                if (disposable != null) disposable.Dispose();
            }
        }
    }
}