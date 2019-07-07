using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public static class ExtensionMethods
    {
        public static bool TryGet<T>(this IResourceHandler<T> resourceHandler, out T result, T defaultValue = default)
        {
            if (resourceHandler == null)
            {
                result = defaultValue;
                return true;
            }

            if (resourceHandler.Status == TaskStatus.RanToCompletion)
            {
                result = resourceHandler.GetAsync().Result;
                return true;
            }

            result = defaultValue;
            return false;
        }

        public static bool TryGetAs<T, V>(this IResourceHandler<T> resourceHandler, out V result,
            V defaultValue = default) where V : class
        {
            if (resourceHandler == null)
            {
                result = defaultValue;
                return true;
            }

            if (resourceHandler.Status == TaskStatus.RanToCompletion)
            {
                result = resourceHandler.GetAsync().Result as V;
                return true;
            }

            result = defaultValue;
            return false;
        }

        public static async Task<T> GetAsyncOrDefault<T>(this IResourceHandler<T> resourceHandler,
            T defaultValue = default)
        {
            if (resourceHandler == null) return defaultValue;

            try
            {
                return await resourceHandler.GetAsync();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to load " + resourceHandler + ": " + ex);
                return defaultValue;
            }
        }

        public static ResourceManager With<T>(this ResourceManager resourceManager, IResourceCollection<T> collection)
        {
            resourceManager.Register(collection);
            return resourceManager;
        }

        public static ResourceManager With<T>(this ResourceManager resourceManager, ResourceLoader<T> loader)
        {
            resourceManager.Register(loader);
            return resourceManager;
        }

        public static ResourceManager With(this ResourceManager resourceManager,
            ResourceLoader<IResourceContainer> loader, params string[] extensions)
        {
            resourceManager.Register(loader, extensions);
            return resourceManager;
        }
    }
}