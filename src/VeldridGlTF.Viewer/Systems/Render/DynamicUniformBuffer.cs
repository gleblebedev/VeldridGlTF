using System;
using System.Runtime.InteropServices;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class DynamicUniformBuffer: IDisposable
    {
        private readonly uint _localBufferSizeInBytes;
        private uint _offsetAlignment;
        private DeviceBuffer _buffer;
        private uint _sizeInBytes;
        private uint _position;
        private uint _uncommitedPosition;
        private byte[] _localBuffer;
        private GraphicsDevice _graphicsDevice;

        public DynamicUniformBuffer(RenderContext renderContext, uint sizeInBytes, byte[] localBuffer)
        {
            _localBuffer = localBuffer;
            _graphicsDevice = renderContext.Device;
            _offsetAlignment = _graphicsDevice.UniformBufferMinOffsetAlignment;
            _sizeInBytes = GetAlignedSize(sizeInBytes);
            _buffer = renderContext.Factory.CreateBuffer(new BufferDescription(sizeInBytes,
                BufferUsage.UniformBuffer | BufferUsage.Dynamic));
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
                _position = 0;
                _uncommitedPosition = 0;
            }

            if (_position + alignedSizeInBytes > _localBuffer.Length)
            {
                Commit();
            }
            var pos = _position;
            _position += alignedSizeInBytes;
            return pos;
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
            _graphicsDevice.UpdateBuffer(_buffer, _uncommitedPosition, _localBuffer);
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