using System;
using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace VeldridGlTF.Viewer
{
    public class VeldridStartupWindow : IApplicationWindow
    {
        private readonly ViewerOptions _options;
        private readonly Sdl2Window _window;
        private DisposeCollectorResourceFactory _factory;
        private GraphicsDevice _gd;
        private bool _windowResized = true;

        public VeldridStartupWindow(string title, ViewerOptions options)
        {
            _options = options;
            var wci = new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = 1280,
                WindowHeight = 720,
                WindowTitle = title,
                WindowInitialState = _options.WindowState
            };
            _window = VeldridStartup.CreateWindow(ref wci);
            _window.Resized += () => { _windowResized = true; };
            _window.KeyDown += OnKeyDown;
        }

        public event Action<float> Rendering;
        public event Action<GraphicsDevice, ResourceFactory, Swapchain> GraphicsDeviceCreated;
        public event Action GraphicsDeviceDestroyed;
        public event Action Resized;

        public uint Width => (uint) _window.Width;
        public uint Height => (uint) _window.Height;

        public SamplePlatformType PlatformType => SamplePlatformType.Desktop;

        public void Run()
        {
            var options = new GraphicsDeviceOptions(
                false,
                PixelFormat.R16_UNorm,
                true,
                ResourceBindingModel.Improved,
                true,
                true);

#if DEBUG
            options.Debug = true;
#endif
            if (_options.GraphicsBackend.HasValue)
                _gd = VeldridStartup.CreateGraphicsDevice(_window, options, _options.GraphicsBackend.Value);
            else
                _gd = VeldridStartup.CreateGraphicsDevice(_window, options);
            _factory = new DisposeCollectorResourceFactory(_gd.ResourceFactory);
            GraphicsDeviceCreated?.Invoke(_gd, _factory, _gd.MainSwapchain);

            var sw = Stopwatch.StartNew();
            var previousElapsed = sw.Elapsed.TotalSeconds;

            while (_window.Exists)
            {
                var newElapsed = sw.Elapsed.TotalSeconds;
                var deltaSeconds = (float) (newElapsed - previousElapsed);

                var inputSnapshot = _window.PumpEvents();

                if (_window.Exists)
                {
                    previousElapsed = newElapsed;
                    if (_windowResized)
                    {
                        _windowResized = false;
                        _gd.ResizeMainWindow((uint) _window.Width, (uint) _window.Height);
                        Resized?.Invoke();
                    }

                    Rendering?.Invoke(deltaSeconds);
                }
            }

            _gd.WaitForIdle();
            _factory.DisposeCollector.DisposeAll();
            _gd.Dispose();
            GraphicsDeviceDestroyed?.Invoke();
        }

        public event Action<KeyEvent> KeyPressed;

        protected void OnKeyDown(KeyEvent keyEvent)
        {
            KeyPressed?.Invoke(keyEvent);
        }
    }
}