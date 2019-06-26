﻿using System;
using Veldrid;

namespace VeldridGlTF.Viewer
{
    public interface IApplicationWindow
    {
        SamplePlatformType PlatformType { get; }

        event Action<float> Rendering;
        event Action<GraphicsDevice, ResourceFactory, Swapchain> GraphicsDeviceCreated;
        event Action GraphicsDeviceDestroyed;
        event Action Resized;

        uint Width { get; }
        uint Height { get; }

        void Run();
    }
}