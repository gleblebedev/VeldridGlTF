using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderContext
    {
        public RenderContext(GraphicsDevice device, ResourceFactory factory, Swapchain swapchain,
            VeldridRenderSystem renderSystem)
        {
            Device = device;
            Factory = factory;
            Swapchain = swapchain;
            RenderSystem = renderSystem;
        }

        public GraphicsDevice Device { get; }

        public ResourceFactory Factory { get; }

        public Swapchain Swapchain { get; }

        public VeldridRenderSystem RenderSystem { get; }
    }
}