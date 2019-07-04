using System;

namespace VeldridGlTF.Viewer.Resources
{
    public struct ResourceId : IEquatable<ResourceId>
    {
        public static readonly ResourceId Null = new ResourceId(null, null);

        public ResourceId(string container, string id)
        {
            Container = container;
            Id = id;
        }

        public string Container { get; }

        public string Id { get; }

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

        public override string ToString()
        {
            return $"{Container}/{Id}";
        }
    }
}