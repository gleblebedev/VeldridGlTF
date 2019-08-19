using VeldridGlTF.Viewer.Systems.Render.Uniforms;

namespace VeldridGlTF.Viewer.Systems.Render.Buffers
{
    public class ObjectPropertiesBuffer : DynamicUniformBuffer<ObjectProperties>
    {
        public ObjectPropertiesBuffer(RenderContext renderContext, uint sizeInBytes, byte[] localBuffer)
            :base(renderContext, sizeInBytes, localBuffer)
        {
            
        }

        public ref ObjectProperties Allocate(out uint offset)
        {
            offset = base.Allocate(_elementSize);
            uint localOffset = offset - _uncommitedPosition;
            unsafe
            {
                fixed (byte* ptr = _localBuffer)
                {
                    return ref *(ObjectProperties*) (ptr + localOffset);
                }
            }
        }
    }
}