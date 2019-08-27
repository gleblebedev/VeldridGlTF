using System;

namespace VeldridGlTF.Viewer.Resources
{
    public class AbstractResource : IDisposable
    {
        public AbstractResource(ResourceId id)
        {
            Id = id;
        }

        public ResourceId Id { get; }

        public virtual void Dispose()
        {
        }

        public override string ToString()
        {
            return $"{Id.Path}#{Id.Id}<{GetType().Name}>";
        }
    }
}