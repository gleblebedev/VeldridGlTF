using CommandLine;
using Veldrid;

namespace VeldridGlTF.Viewer
{
    public class ViewerOptions
    {
        [Option('g', "graphics")] public GraphicsBackend? GraphicsBackend { get; set; }

        [Option('w', "windowstate")] public WindowState WindowState { get; set; } = WindowState.Normal;
    }
}