using  Veldrid;

namespace VeldridGlTF.Viewer
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            Veldrid.Sdl2.SDL_version version;
            Veldrid.Sdl2.Sdl2Native.SDL_GetVersion(&version);
            new GlTFViewer().Run();
        }
    }
}
