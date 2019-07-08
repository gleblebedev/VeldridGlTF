using System;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public class ManualResourceHandler<T> : IResourceHandler<T>
    {
        private TaskCompletionSource<T> _taskSource;

        public ManualResourceHandler(ResourceId id)
        {
            Id = id;
            _taskSource = new TaskCompletionSource<T>();
        }

        public ManualResourceHandler(ResourceId id, T value) : this(id)
        {
            _taskSource.SetResult(value);
        }

        public ManualResourceHandler(ResourceId id, Exception ex) : this(id)
        {
            _taskSource.SetException(ex);
        }

        public Task<T> GetAsync()
        {
            if (_taskSource == null) throw CreateObjectDisposedException();
            return _taskSource.Task;
        }

        public void Dispose()
        {
            _taskSource.TrySetException(new TaskCanceledException());
            _taskSource = null;
        }

        public ResourceId Id { get; }

        public event EventHandler ResourceChanged;

        public TaskStatus Status
        {
            get
            {
                if (_taskSource == null) return TaskStatus.Canceled;
                return _taskSource.Task.Status;
            }
        }

        public void SetValue(T value)
        {
            if (_taskSource == null) throw CreateObjectDisposedException();
            if (_taskSource.TrySetResult(value)) return;
            _taskSource = new TaskCompletionSource<T>();
            _taskSource.SetResult(value);
            ResourceChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetException(Exception value)
        {
            if (_taskSource == null) throw CreateObjectDisposedException();
            if (_taskSource.TrySetException(value)) return;
            _taskSource = new TaskCompletionSource<T>();
            _taskSource.SetException(value);
            ResourceChanged?.Invoke(this, EventArgs.Empty);
        }

        private static ObjectDisposedException CreateObjectDisposedException()
        {
            return new ObjectDisposedException("this",
                string.Format("ManualResourceHandler<{1}> is already disposed", typeof(T).Name));
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public void Reset()
        {
            if (_taskSource == null) throw CreateObjectDisposedException();
            _taskSource.TrySetCanceled();
            _taskSource = new TaskCompletionSource<T>();
            ResourceChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}