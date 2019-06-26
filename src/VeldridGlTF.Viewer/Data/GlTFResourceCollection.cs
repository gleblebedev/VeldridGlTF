using System.Collections;
using System.Collections.Generic;

namespace VeldridGlTF.Viewer.Data
{
    public class GlTFResourceCollection<R,T>:IEnumerable<T>
    {
        private Dictionary<R, T> _byType = new Dictionary<R, T>();
        private Dictionary<string, T> _byId = new Dictionary<string, T>();

        public T this[string id]
        {
            get
            {
                T res;
                if (!_byId.TryGetValue(id, out res))
                    return default(T);
                return res;
            }
        }
        public T this[R id]
        {
            get
            {
                T res;
                if (!_byType.TryGetValue(id, out res))
                    return default(T);
                return res;
            }
        }

        public void Add(R res, string id, T val)
        {
            _byType.Add(res, val);
            _byId.Add(id, val);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _byType.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}