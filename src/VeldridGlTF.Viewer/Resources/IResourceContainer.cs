namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceContainer
    {
        IResourceHandler<T> Resolve<T>(ResourceId id);
    }
}