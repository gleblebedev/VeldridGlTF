using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderContext
    {
        private readonly GraphicsDevice _device;
        private readonly ResourceFactory _factory;
        private readonly Swapchain _swapchain;

        public RenderContext(GraphicsDevice device, ResourceFactory factory, Swapchain swapchain)
        {
            _device = device;
            _factory = factory;
            _swapchain = swapchain;
        }

        public GraphicsDevice Device
        {
            get { return _device; }
        }

        public ResourceFactory Factory
        {
            get { return _factory; }
        }

        public Swapchain Swapchain1
        {
            get { return _swapchain; }
        }
    }
}