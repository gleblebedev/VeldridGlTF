using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace VeldridGlTF.Viewer.Data
{
    public class GlTFResourceCollection<R, T> : IEnumerable<KeyValuePair<R, T>> where R : class
    {
        private readonly Dictionary<string, T> _byId = new Dictionary<string, T>();
        private readonly Dictionary<R, T> _byType = new Dictionary<R, T>();
        private T _nullId;

        public T this[string id]
        {
            get
            {
                if (id == null) return _nullId;
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

        public IEnumerator<KeyValuePair<R, T>> GetEnumerator()
        {
            return _byType.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(R res, string id, T val)
        {
            if (res != null)
                _byType.Add(res, val);
            if (id == null)
                _nullId = val;
            else
            {
                if (_byId.ContainsKey(id))
                {
                    Debug.WriteLine("Duplicate resource"+id);
                }
                else
                {
                    _byId.Add(id, val);
                }
            }
        }

        public bool ContainsId(string id)
        {
            if (id == null)
                return _nullId != null;
            return _byId.ContainsKey(id);
        }
    }
}