using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Veldrid;

namespace VeldridGlTF.Viewer.Android
{
    [Activity(
        MainLauncher = true,
        Label = "Veldrid.glTF.Viewer",
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize
    )]
    public class MainActivity : Activity
    {
        private VeldridSurfaceView _view;
        private AndroidApplicationWindow _window;
        private SceneRenderer _tc;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            bool debug = false;
//#if DEBUG
//            debug = true;
//#endif

            GraphicsDeviceOptions options = new GraphicsDeviceOptions(
                debug,
                PixelFormat.R16_UNorm,
                false,
                ResourceBindingModel.Improved,
                true,
                true);
            var viewerOptions = new ViewerOptions();
            //viewerOptions.GraphicsBackend = GraphicsBackend.OpenGLES;

            _view = new VeldridSurfaceView(this, viewerOptions, options);
            _window = new AndroidApplicationWindow(this, _view);
            _window.GraphicsDeviceCreated += (g, r, s) => _window.Run();
            _tc = new SceneRenderer(_window);
            SetContentView(_view);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _view.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _view.OnResume();
        }
    }
}

