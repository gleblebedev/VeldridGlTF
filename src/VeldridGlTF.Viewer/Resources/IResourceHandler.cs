using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceHandler<T> : IResourceHandler
    {
        new Task<T> GetAsync();
    }

    public interface IResourceHandler
    {
        TaskStatus Status { get; }

        Task GetAsync();
    }
}