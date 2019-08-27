using System;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class PipelineBinder
    {
        public Pipeline Pipeline { get; set; }

        public ResourceLayout[] ResourceLayouts { get; set; }

        public ResourceSet[] Sets { get; set; }

        public DynamicResource[][] DynamicOffsets { get; set; }

        public void Set(CommandList cl, uint objectProperties, uint jointMatrices, uint jointNormalMatrices)
        {
            cl.SetPipeline(Pipeline);
            for (var index = 0; index < Sets.Length; index++)
            {
                var resourceSet = Sets[index];
                if (resourceSet != null)
                {
                    var offsets = DynamicOffsets[index];
                    if (offsets != null)
                    {
                        var offsetBuf = new uint[offsets.Length];
                        for (var i = 0; i < offsets.Length; i++)
                            switch (offsets[i])
                            {
                                case DynamicResource.ObjectProperties:
                                    offsetBuf[i] = objectProperties;
                                    break;
                                case DynamicResource.JointMatrices:
                                    offsetBuf[i] = jointMatrices;
                                    break;
                                case DynamicResource.JointNormalMatrices:
                                    offsetBuf[i] = jointNormalMatrices;
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }

                        cl.SetGraphicsResourceSet((uint) index, resourceSet, offsetBuf);
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