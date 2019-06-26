using System.Collections;
using System.Collections.Generic;

namespace VeldridGlTF.Viewer.Data
{
    public class GlTFResourceCollection<R, T> : IEnumerable<T> where R : class
    {
        private readonly Dictionary<string, T> _byId = new Dictionary<string, T>();
        private readonly Dictionary<R, T> _byType = new Dictionary<R, T>();

        public T this[string id]
        {
            get
            {
                if (id == null) return default;
                T res;
                if (!_byId.TryGetValue(id, out res))
                    return default;
                return res;
            }
        }

        public T this[R id]
        {
            get
            {
                if (id == null) return default;
                T res;
                if (!_byType.TryGetValue(id, out res))
                    return default;
                return res;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _byType.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(R res, string id, T val)
        {
            _byType.Add(res, val);
            _byId.Add(id, val);
        }

        public bool ContainsId(string id)
        {
            return _byId.ContainsKey(id);
        }
    }
}