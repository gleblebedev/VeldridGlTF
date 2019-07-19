using CommandLine;
using VeldridGlTF.Viewer.Loaders.FileSystem;

namespace VeldridGlTF.Viewer.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<ViewerOptions>(args) as Parsed<ViewerOptions>;

            VeldridStartupWindow window = new VeldridStartupWindow("glTF Viewer", options?.Value ?? new ViewerOptions());
            //var valueRootFolder = new FilesystemFolder("Assets", "");
            //valueRootFolder.Mount("buster_drone", new ZipMount(@"Assets\buster_drone.zip", "buster_drone"));
            //options.Value.RootFolder = valueRootFolder;
            //options.Value.FileName = "buster_drone/scene.gltf";
            SceneRenderer sceneRenderer = new SceneRenderer(window, options.Value);
            window.Run();
        }
    }
}
