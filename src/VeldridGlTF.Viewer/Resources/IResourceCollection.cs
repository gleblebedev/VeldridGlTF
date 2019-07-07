namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceCollection<T> : IResourceCollection
    {
        IResourceHandler<T> Resolve(ResourceId id);
    }

    public interface IResourceCollection
    {
    }
}