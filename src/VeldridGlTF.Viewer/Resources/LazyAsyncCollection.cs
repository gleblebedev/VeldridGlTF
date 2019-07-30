using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public class LazyAsyncCollection<TKey, TValue>
    {
        private readonly Func<TKey, Lazy<Task<TValue>>> _factory;

        private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _resources =
            new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();

        public LazyAsyncCollection(Func<TKey, Task<TValue>> factory)
        {
            _factory = _=>new Lazy<Task<TValue>>(()=>factory(_), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public LazyAsyncCollection(Func<TKey, TValue> factory): this(_ => Task.Run(() => factory(_)))
        {
        }
        public Task<TValue> this[TKey key]
        {
            get { return _resources.GetOrAdd(key, _factory).Value; }
        }
    }
}