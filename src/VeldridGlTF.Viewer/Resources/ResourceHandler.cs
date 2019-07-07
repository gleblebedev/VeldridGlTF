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
        private readonly ResourceLoader<T> _loader;
        private readonly ResourceManager _manager;
        private ResourceContext<T> _context;

        public ResourceHandler(ResourceId id, ResourceLoader<T> loader, ResourceManager manager) : base(id)
        {
            _loader = loader;
            _manager = manager;
            _gate = new object();
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

                        _context = value;

                        if (_context != null)
                        {
                            _context.ResourceInvalid += OnResourceInvalid;
                            Start();
                        }
                    }
            }
        }

        public override TaskStatus Status => GetAsync().Status;

        public Task<T> GetAsync()
        {
            if (_context == null)
                lock (_gate)
                {
                    if (_context == null)
                    {
                        Context = new ResourceContext<T>(Id, _manager, _loader);
                        return _context.Task;
                    }
                }

            return _context.Task;
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
    }
}