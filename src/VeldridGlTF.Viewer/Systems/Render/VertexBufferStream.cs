using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class VertexBufferStream
    {
        private byte[] _data;

        public VertexBufferStream() : this(1024)
        {
        }

        public VertexBufferStream(int capacity)
        {
            _data = new byte[capacity];
            Position = 0;
        }

        public int Position { get; private set; }

        public unsafe void Write(float value)
        {
            var index = Allocate(4);
            var TmpValue = *(uint*) &value;
            _data[index] = (byte) TmpValue;
            _data[index + 1] = (byte) (TmpValue >> 8);
            _data[index + 2] = (byte) (TmpValue >> 16);
            _data[index + 3] = (byte) (TmpValue >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Allocate(int size)
        {
            var p = Position;
            if (_data.Length < Position + size)
            {
                var buf = new byte[Math.Max(_data.Length * 2, Position + size)];
                _data.CopyTo(buf, 0);
                _data = buf;
            }

            Position += size;

            return p;
        }

        public byte[] ToArray()
        {
            if (Position == _data.Length)
                return _data;
            return new ArraySegment<byte>(_data, 0, Position).ToArray();
        }
    }
}