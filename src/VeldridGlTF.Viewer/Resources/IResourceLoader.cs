using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceLoader<T> : IResourceLoader
    {
        Task<T> LoadAsync(ResourceManager manager, ResourceId id);
    }

    public interface IResourceLoader
    {
    }
}