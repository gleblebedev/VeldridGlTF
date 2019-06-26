namespace VeldridGlTF.Viewer.Resources
{
    public static class ExtensionMethod
    {
        public static ResourceManager With<T>(this ResourceManager resourceManager, IResourceLoader<T> loader)
        {
            resourceManager.Register<T>(loader);
            return resourceManager;
        }
    }
}