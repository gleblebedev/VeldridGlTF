using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public abstract class ResourceLoader<T>
    {
        public abstract Task<T> LoadAsync(ResourceContext context);
    }
}