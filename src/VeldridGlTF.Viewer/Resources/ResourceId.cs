using System;

namespace VeldridGlTF.Viewer.Resources
{
    public struct ResourceId : IEquatable<ResourceId>
    {
        public static readonly ResourceId Null = new ResourceId(null, null);

        public string Container
        {
            get { return _container; }
        }

        public string Id
        {
            get { return _id; }
        }

        public bool Equals(ResourceId other)
        {
            return string.Equals(Container, other.Container) && string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Container != null ? Container.GetHashCode() : 0) * 397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ResourceId left, ResourceId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResourceId left, ResourceId right)
        {
            return !left.Equals(right);
        }

        private readonly string _container;
        private readonly string _id;

        public ResourceId(string container, string id)
        {
            _container = container;
            _id = id;
        }
    }
}