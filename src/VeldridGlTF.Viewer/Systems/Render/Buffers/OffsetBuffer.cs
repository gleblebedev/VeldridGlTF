using System;
using System.Runtime.CompilerServices;

namespace VeldridGlTF.Viewer.Systems.Render.Buffers
{
    public class OffsetBuffer
    {
        private readonly uint[] _offsets;

        private uint _position;
        public OffsetBuffer(uint capacity)
        {
            _offsets = new uint[capacity];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Add(uint offset)
        {
            var position = _position;
            if (position == _offsets.Length)
                throw new IndexOutOfRangeException("Offset buffer overflow");
            _offsets[position] = offset;
            ++_position;
            return position;
        }

        public uint this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _offsets[index]; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _offsets[index] = value; }
        }

        public uint Allocate(uint count)
        {
            var position = _position;
            if (position+ count > _offsets.Length)
                throw new IndexOutOfRangeException("Offset buffer overflow");
            _position += count;
            return position;
        }

        public ref uint GetRefAt(uint pos)
        {
            return ref _offsets[pos];
        }

        public void Reset()
        {
            _position = 0;
        }
    }
}