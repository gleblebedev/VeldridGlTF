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

        public void Set(CommandList cl, VeldridRenderSystem veldridRenderSystem, MaterialResource material)
        {
            cl.SetPipeline(_pipeline);
            for (var index = 0; index < Sets.Length; index++)
            {
                var resourceSet = Sets[index];
                if (resourceSet != null)
                    cl.SetGraphicsResourceSet((uint)index, resourceSet);
            }
            //cl.SetGraphicsResourceSet(0, veldridRenderSystem.EnvironmentSet);
            //cl.SetGraphicsResourceSet(1, veldridRenderSystem.ZoneSet);
            //cl.SetGraphicsResourceSet(2, veldridRenderSystem.MeshSet);
            //cl.SetGraphicsResourceSet(3, material.ResourceSetBuilder);

        }
    }
}