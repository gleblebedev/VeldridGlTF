namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceContainer
    {
        IResourceHandler Resolve(ResourceId id);
    }
}