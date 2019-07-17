using System;
using System.Threading;
using System.Threading.Tasks;

namespace VeldridGlTF.Viewer.Resources
{
    public interface IResourceHandler<T> : IResourceHandler
    {
        Task<T> GetAsync();
    }

    public interface IResourceHandler : IDisposable
    {
        ResourceId Id { get; }

        TaskStatus Status { get; }

        event EventHandler ResourceChanged;
    }
}