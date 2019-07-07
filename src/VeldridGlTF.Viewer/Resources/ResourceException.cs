using System;

namespace VeldridGlTF.Viewer.Resources
{
    public class ResourceException : Exception
    {
        public ResourceException(ResourceId id, string message) : base(message)
        {
            ResourceId = id;
        }

        public ResourceException(ResourceId id, Exception ex) : base(string.Format("Failed to load resource {0}", id),
            ex)
        {
            ResourceId = id;
        }

        public ResourceId ResourceId { get; }
    }
}