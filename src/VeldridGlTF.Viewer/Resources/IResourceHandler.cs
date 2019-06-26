using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceHandler<T>: IResourceHandler
    {
        Task<T> GetAsync();
    }

    public interface IResourceHandler
    {
        TaskStatus Status { get; }
    }
}