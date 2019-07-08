using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceLoader<T>
    {
        Task<T> LoadAsync(ResourceContext context);
    }
}