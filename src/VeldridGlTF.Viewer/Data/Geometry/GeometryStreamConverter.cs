using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public abstract class GeometryStreamConverter<T1, T2> : IReadOnlyList<T2>
    {
        private readonly IReadOnlyList<T1> _data;

        public GeometryStreamConverter(IReadOnlyList<T1> data)
        {
            _data = data;
        }

        public IEnumerator<T2> GetEnumerator()
        {
            foreach (var f in _data) yield return Convert(f);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _data.Count;

        public T2 this[int index] => Convert(_data[index]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract T2 Convert(T1 value);
    }
}