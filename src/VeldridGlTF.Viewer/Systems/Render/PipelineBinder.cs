using System;
using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class PipelineBinder
    {
        public PipelineBinder()
        {
        }

        private Pipeline _pipeline;
        private ResourceLayout[] _resourceLayouts;
        private ResourceSet[] _sets;
        private DynamicResource[][] _dynamicOffsets;

        public Pipeline Pipeline
        {
            get { return _pipeline; }
            set { _pipeline = value; }
        }

        public ResourceLayout[] ResourceLayouts
        {
            get { return _resourceLayouts; }
            set { _resourceLayouts = value; }
        }

        public ResourceSet[] Sets
        {
            get { return _sets; }
            set { _sets = value; }
        }

        public DynamicResource[][] DynamicOffsets
        {
            get { return _dynamicOffsets; }
            set { _dynamicOffsets = value; }
        }

        public void Set(CommandList cl, uint objectProperties)
        {
            cl.SetPipeline(_pipeline);
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
                        {
                            switch (offsets[i])
                            {
                                case DynamicResource.ObjectProperties:
                                    offsetBuf[i] = objectProperties;
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        cl.SetGraphicsResourceSet((uint)index, resourceSet, offsetBuf);
                    }
                    else
                    {
                        cl.SetGraphicsResourceSet((uint)index, resourceSet);
                    }
                }
            }
        }
    }
}