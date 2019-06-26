namespace VeldridGlTF.Viewer.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            VeldridStartupWindow window = new VeldridStartupWindow("glTF Viewer");
            SceneRenderer sceneRenderer = new SceneRenderer(window);
            window.Run();
        }
    }
}
