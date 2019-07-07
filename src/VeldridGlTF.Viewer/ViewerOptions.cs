using CommandLine;
using Veldrid;
using VeldridGlTF.Viewer.Loaders.FileSystem;

namespace VeldridGlTF.Viewer
{
    public class ViewerOptions
    {
        [Option('g', "graphics")] public GraphicsBackend? GraphicsBackend { get; set; }

        [Option('w', "windowstate")] public WindowState WindowState { get; set; } = WindowState.Normal;

        [Option('d', "data", Default = "Assets")]
        public string DataFolder { get; set; } = "Assets";

        [Option('i', "input")] public string FileName { get; set; }

        [Option('s', "scale", Default = 1.0f)] public float Scale { get; set; } = 1.0f;

        public IFolder RootFolder { get; set; }
    }
}