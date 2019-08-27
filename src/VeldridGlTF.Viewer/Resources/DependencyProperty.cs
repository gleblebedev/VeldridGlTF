using System;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public class DependencyProperty<T> : IResourceHandler<T>
    {
        private readonly DependencyPropertyFlags _flags;
        private readonly object _gate = new object();
        private IResourceHandler<T> _parent;
        private Task<T> _task;

        public DependencyProperty(DependencyPropertyFlags flags = DependencyPropertyFlags.StickyValue)
        {
            _task = Task.FromResult(default(T));
            _flags = flags;
        }

        public IResourceHandler<T> Parent
        {
            get => _parent;
            private set
            {
                if (_parent == value) return;

                lock (_gate)
                {
                    if (_parent == value) return;

                    if (_parent != null) _parent.ResourceChanged -= OnResourceChanged;

                    _parent = value;

                    if (_parent != null) _parent.ResourceChanged += OnResourceChanged;
                }
            }
        }

        public Task<T> GetAsync()
        {
            return _task;
        }

        public void Dispose()
        {
            SetValue(default(T));
        }

        public ResourceId Id => _parent?.Id ?? ResourceId.Null;

        public TaskStatus Status => _task.Status;

        public event EventHandler ResourceChanged;

        private void OnResourceChanged(object sender, EventArgs e)
        {
            FetchValueFromParent((IResourceHandler<T>) sender);
        }

        public void SetValue(IResourceHandler<T> handler)
        {
            Parent = handler;
            FetchValueFromParent(handler);
        }

        private void FetchValueFromParent(IResourceHandler<T> handler)
        {
            if (handler != Parent) return;

            lock (_gate)
            {
                if (handler != Parent) return;

                var task = handler.GetAsync();
                if (task.Status == TaskStatus.RanToCompletion || task.Status == TaskStatus.Faulted ||
                    task.Status == TaskStatus.Canceled)
                {
                    _task = task;
                }
                else
                {
                    if (DependencyPropertyFlags.StickyValue == (_flags & DependencyPropertyFlags.StickyValue))
                        task.ContinueWith(_ => SetTaskResult(_, handler));
                    else
                        _task = task;
                }
            }

            ResourceChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetValue(T value)
        {
            lock (_gate)
            {
                _task = Task.FromResult(value);
                ResourceChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetTaskResult(Task<T> sourceTask, IResourceHandler<T> parent)
        {
            if (_parent == parent)
                lock (_gate)
                {
                    if (_parent == parent)
                    {
                        _task = sourceTask;
                        ResourceChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}