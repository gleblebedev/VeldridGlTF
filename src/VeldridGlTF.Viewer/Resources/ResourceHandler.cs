using System;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public abstract class ResourceHandler : IResourceHandler
    {
        public ResourceHandler(ResourceId id)
        {
            Id = id;
        }

        public ResourceId Id { get; }

        public event EventHandler ResourceChanged;

        public virtual void Dispose()
        {
        }

        public abstract TaskStatus Status { get; }


        protected void RaiseResourceChanged()
        {
            ResourceChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ResourceHandler<T> : ResourceHandler, IResourceHandler<T>
    {
        private readonly object _gate;
        private readonly IResourceLoader<T> _loader;
        private readonly IResourceContainer _resourceContainer;
        private ResourceContext<T> _context;

        public ResourceHandler(ResourceId id, IResourceLoader<T> loader, IResourceContainer resourceContainer) :
            base(id)
        {
            _loader = loader;
            _resourceContainer = resourceContainer;
            _gate = new object();
        }

        public ResourceHandler(ResourceId id, Func<ResourceContext, Task<T>> loader,
            IResourceContainer resourceContainer) : this(id, new Loader(loader), resourceContainer)
        {
        }

        private ResourceContext<T> Context
        {
            get => _context;
            set
            {
                if (_context != value)
                    lock (_gate)
                    {
                        if (_context != null)
                        {
                            _context.ResourceInvalid -= OnResourceInvalid;
                            _context.Dispose();
                        }

                        if (value != null)
                        {
                            value.ResourceInvalid += OnResourceInvalid;
                            value.Start();
                        }

                        // Assign _context value after context.Start() is executed.
                        _context = value;
                    }
            }
        }

        public override TaskStatus Status => GetAsync().Status;

        public Task<T> GetAsync()
        {
            var resourceContext = _context;
            if (resourceContext == null)
                lock (_gate)
                {
                    resourceContext = _context;
                    if (resourceContext == null)
                    {
                        Context = resourceContext = new ResourceContext<T>(Id, _resourceContainer, _loader);
                        return resourceContext.Task;
                    }
                }

            return resourceContext.Task;
        }

        public override void Dispose()
        {
            base.Dispose();

            Context = null;
        }

        private void Start()
        {
            _context.Start();
        }

        private void OnResourceInvalid(object sender, EventArgs e)
        {
            Invalidate();
        }


        public override string ToString()
        {
            return string.Format("{0}<{1}>", Id, typeof(T).Name);
        }

        public void Invalidate()
        {
            lock (_gate)
            {
                Context = null;
                RaiseResourceChanged();
            }
        }

        private class Loader : IResourceLoader<T>
        {
            private readonly Func<ResourceContext, Task<T>> _func;

            public Loader(Func<ResourceContext, Task<T>> func)
            {
                _func = func;
            }

            public Task<T> LoadAsync(ResourceContext context)
            {
                return _func(context);
            }
        }
    }
}