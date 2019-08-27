using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.Buffers
{
    public class DynamicUniformBuffer<T> : DynamicUniformBuffer where T : struct
    {
        protected readonly uint _elementSize;

        public DynamicUniformBuffer(RenderContext renderContext, uint sizeInBytes, byte[] localBuffer)
            : base(renderContext, (uint) Marshal.SizeOf<T>(), sizeInBytes, localBuffer)
        {
        }

        public uint Allocate(out ArraySegment<byte> segment)
        {
            return Allocate(_elementSize, out segment);
        }
    }

    public class DynamicUniformBuffer : IDisposable
    {
        private readonly CommandList _commandList;
        private readonly uint _localBufferSizeInBytes;
        private readonly GraphicsDevice _graphicsDevice;
        protected byte[] _localBuffer;
        private uint _position;
        protected uint _uncommitedPosition;

        public DynamicUniformBuffer(RenderContext renderContext, uint elementSize, uint sizeInBytes, byte[] localBuffer)
        {
            ElementSize = elementSize;
            _localBuffer = localBuffer;
            _graphicsDevice = renderContext.Device;
            OffsetAlignment = _graphicsDevice.UniformBufferMinOffsetAlignment;
            SizeInBytes = GetAlignedSize(sizeInBytes);
            DeviceBuffer = renderContext.Factory.CreateBuffer(new BufferDescription(sizeInBytes,
                BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            BindableResource = new DeviceBufferRange(DeviceBuffer, 0, GetAlignedSize(ElementSize));
        }

        public DeviceBufferRange BindableResource { get; }

        public DeviceBuffer DeviceBuffer { get; }


        public uint ElementSize { get; }


        public uint OffsetAlignment { get; }

        public uint SizeInBytes { get; }

        public void Dispose()
        {
            DeviceBuffer.Dispose();
        }

        public uint Allocate(uint sizeInBytes)
        {
            var alignedSizeInBytes = GetAlignedSize(sizeInBytes);
            if (alignedSizeInBytes > _localBuffer.Length)
                throw new IndexOutOfRangeException("Can't allocate more than local buffer size.");
            if (_position + alignedSizeInBytes > SizeInBytes)
            {
                Commit();
                Reset();
            }

            if (_position + alignedSizeInBytes > _localBuffer.Length) Commit();
            var pos = _position;
            _position += alignedSizeInBytes;

            return pos;
        }

        public uint Allocate(uint sizeInBytes, out ArraySegment<byte> segment)
        {
            var pos = Allocate(sizeInBytes);
            segment = new ArraySegment<byte>(_localBuffer, (int) (pos - _uncommitedPosition), (int) sizeInBytes);
            return pos;
        }

        public void Reset()
        {
            _position = 0;
            _uncommitedPosition = 0;
        }

        public uint Add<T>(ref T value) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var pos = Allocate((uint)size);
            SetAt(pos, ref value);
            return pos;
        }

        public void Commit()
        {
            if (_position == _uncommitedPosition)
                return;
            if (_commandList != null)
                _commandList.UpdateBuffer(DeviceBuffer, _uncommitedPosition, ref _localBuffer[0],
                    _position - _uncommitedPosition);
            else
                _graphicsDevice.UpdateBuffer(DeviceBuffer, _uncommitedPosition, ref _localBuffer[0],
                    _position - _uncommitedPosition);
            _uncommitedPosition = _position;
        }

        public uint GetAlignedSize(uint sizeInBytes)
        {
            var alignedSizeInBlocks = (sizeInBytes + OffsetAlignment - 1) / OffsetAlignment;
            return alignedSizeInBlocks * OffsetAlignment;
        }

        public void SetAt<T>(uint pos, ref T value) where T : struct
        {
            var pinStructure = GCHandle.Alloc(value, GCHandleType.Pinned);
            var size = Marshal.SizeOf(typeof(T));
            Marshal.Copy(pinStructure.AddrOfPinnedObject(), _localBuffer, (int)(pos - _uncommitedPosition), size);
            pinStructure.Free();
        }
    }
}