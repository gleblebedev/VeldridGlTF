using CommandLine;

namespace VeldridGlTF.Viewer.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<ViewerOptions>(args) as Parsed<ViewerOptions>;

            VeldridStartupWindow window = new VeldridStartupWindow("glTF Viewer", options?.Value ?? new ViewerOptions());
            SceneRenderer sceneRenderer = new SceneRenderer(window, options.Value);
            window.Run();
        }
    }
}
