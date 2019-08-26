using System;
using System.Runtime.InteropServices;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.Buffers
{
    public class DynamicUniformBuffer<T> : DynamicUniformBuffer where T:struct
    {
        protected readonly uint _elementSize;

        public DynamicUniformBuffer(RenderContext renderContext, uint sizeInBytes, byte[] localBuffer)
            : base(renderContext, (uint)Marshal.SizeOf<T>(), sizeInBytes, localBuffer)
        {
        }

        public uint Allocate(out ArraySegment<byte> segment)
        {
            return Allocate(_elementSize, out segment);
        }
    }

    public class DynamicUniformBuffer: IDisposable
    {
        private readonly uint _localBufferSizeInBytes;
        private uint _offsetAlignment;
        private DeviceBuffer _buffer;
        private uint _sizeInBytes;
        private uint _position;
        protected uint _uncommitedPosition;
        private readonly uint _elementSize;
        protected byte[] _localBuffer;
        private readonly CommandList _commandList;
        private GraphicsDevice _graphicsDevice;
        private readonly DeviceBufferRange _bindableResource;

        public DynamicUniformBuffer(RenderContext renderContext, uint elementSize, uint sizeInBytes, byte[] localBuffer)
        {
            _elementSize = elementSize;
            _localBuffer = localBuffer;
            _graphicsDevice = renderContext.Device;
            _offsetAlignment = _graphicsDevice.UniformBufferMinOffsetAlignment;
            _sizeInBytes = GetAlignedSize(sizeInBytes);
            _buffer = renderContext.Factory.CreateBuffer(new BufferDescription(sizeInBytes,
                BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _bindableResource = new DeviceBufferRange(_buffer, 0, _elementSize);
        }

        public DeviceBufferRange BindableResource { get { return _bindableResource; } }

        public DeviceBuffer DeviceBuffer { get { return _buffer; } }


        public uint ElementSize
        {
            get { return _elementSize; }
        }


        public uint OffsetAlignment 
        {
            get { return _offsetAlignment; }
        }

        public uint SizeInBytes
        {
            get { return _sizeInBytes; }
        }

        public uint Allocate(uint sizeInBytes)
        {
            var alignedSizeInBytes = GetAlignedSize(sizeInBytes);
            if (alignedSizeInBytes > _localBuffer.Length)
            {
                throw new IndexOutOfRangeException("Can't allocate more than local buffer size.");
            }
            if (_position + alignedSizeInBytes > _sizeInBytes)
            {
                Commit();
                Reset();
            }

            if (_position + alignedSizeInBytes > _localBuffer.Length)
            {
                Commit();
            }
            var pos = _position;
            _position += alignedSizeInBytes;

            return pos;
        }
        public uint Allocate(uint sizeInBytes, out ArraySegment<byte> segment)
        {
            var pos = Allocate(sizeInBytes);
            segment = new ArraySegment<byte>(_localBuffer, (int)(pos - _uncommitedPosition), (int)sizeInBytes);
            return pos;
        }
        public void Reset()
        {
            _position = 0;
            _uncommitedPosition = 0;
        }

        public uint Add<T>(ref T value) where T: struct
        {
            GCHandle pinStructure = GCHandle.Alloc(value, GCHandleType.Pinned);
            var size = Marshal.SizeOf(typeof(T));
            var pos = Allocate((uint)size);
            Marshal.Copy(pinStructure.AddrOfPinnedObject(), _localBuffer, (int)(pos - _uncommitedPosition), size);
            pinStructure.Free();
            return pos;
        }

        public void Commit()
        {
            if (_position == _uncommitedPosition)
                return;
            if (_commandList != null)
                _commandList.UpdateBuffer(_buffer, _uncommitedPosition, ref _localBuffer[0], _position - _uncommitedPosition);
            else
                _graphicsDevice.UpdateBuffer(_buffer, _uncommitedPosition, ref _localBuffer[0], _position-_uncommitedPosition);
            _uncommitedPosition = _position;
        }

        public uint GetAlignedSize(uint sizeInBytes)
        {
            var alignedSizeInBlocks = (sizeInBytes + _offsetAlignment - 1) / _offsetAlignment;
            return alignedSizeInBlocks * _offsetAlignment;
        }

        public void Dispose()
        {
            _buffer.Dispose();
        }
    }
}