using System;
using System.Collections;
using System.Collections.Generic;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public struct ListSegment<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _list;
        private readonly int _start;
        private readonly int _count;

        public ListSegment(IList<T> list, int start, int count)
        {
            _list = list;
            _start = start;
            _count = count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_list == null)
                yield break;
            var end = Math.Min(_start + _count, _list.Count);
            for (int i = _start; i < end; ++i)
                yield return _list[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _count; }
        }

        public T this[int index]
        {
            get { return _list[index]; }
        }
    }
}