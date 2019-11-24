using System;
using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.Buffers;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class PipelineBinder
    {
        public Pipeline Pipeline { get; set; }

        public ResourceLayout[] ResourceLayouts { get; set; }

        public ListSegment<ResourceSet> Sets { get; set; }

        public DynamicResource[][] DynamicOffsets { get; set; }

        public void Set(CommandList cl, OffsetBuffer offsetBuf, uint objectProperties, uint jointMatrices, uint jointNormalMatrices)
        {
            cl.SetPipeline(Pipeline);
            for (var index = 0; index < Sets.Count; index++)
            {
                var resourceSet = Sets[index];
                if (resourceSet != null)
                {
                    var dynamicOffsets = DynamicOffsets[index];
                    if (dynamicOffsets != null)
                    {
                        var dynamicOffsetCount = (uint)dynamicOffsets.Length;
                        var pos = offsetBuf.Allocate(dynamicOffsetCount);
                        for (uint i = 0; i < dynamicOffsets.Length; i++)
                            switch (dynamicOffsets[i])
                            {
                                case DynamicResource.ObjectProperties:
                                    offsetBuf[pos+i] = objectProperties;
                                    break;
                                case DynamicResource.JointMatrices:
                                    offsetBuf[pos + i] = jointMatrices;
                                    break;
                                case DynamicResource.JointNormalMatrices:
                                    offsetBuf[pos + i] = jointNormalMatrices;
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }

                        cl.SetGraphicsResourceSet((uint) index, resourceSet, dynamicOffsetCount, ref offsetBuf.GetRefAt(pos));
                    }
                    else
                    {
                        cl.SetGraphicsResourceSet((uint) index, resourceSet);
                    }
                }
            }
        }
    }
}