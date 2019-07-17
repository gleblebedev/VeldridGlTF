using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderContext
    {
        public RenderContext(GraphicsDevice device, ResourceFactory factory, Swapchain swapchain)
        {
            Device = device;
            Factory = factory;
            Swapchain1 = swapchain;
        }

        public GraphicsDevice Device { get; }

        public ResourceFactory Factory { get; }

        public Swapchain Swapchain1 { get; }
    }
}