using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class VertexBufferStream
    {
        private byte[] _data;
        private int _position;
        public VertexBufferStream():this(1024)
        {
        }
        public VertexBufferStream(int capacity)
        {
            _data = new byte[capacity];
            _position = 0;
        }

        public unsafe void Write(float value)
        {
            var index = Allocate(4);
            uint TmpValue = *(uint*)&value;
            _data[index] = (byte)TmpValue;
            _data[index+1] = (byte)(TmpValue >> 8);
            _data[index + 2] = (byte)(TmpValue >> 16);
            _data[index + 3] = (byte)(TmpValue >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Allocate(int size)
        {
            var p = _position;
            if (_data.Length < _position + size)
            {
                var buf = new byte[Math.Max(_data.Length * 2, _position + size)];
                _data.CopyTo(buf,0);
                _data = buf;
            }

            _position += size;

            return p;
        }

        public int Position
        {
            get
            {
                return _position;
            }
        }

        public byte[] ToArray()
        {
            if (_position == _data.Length)
                return _data;
            return (new ArraySegment<byte>(_data, 0, _position)).ToArray();
        }
    }
}