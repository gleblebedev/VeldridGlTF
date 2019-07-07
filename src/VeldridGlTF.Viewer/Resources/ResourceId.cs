using System;

namespace VeldridGlTF.Viewer.Resources
{
    public struct ResourceId : IEquatable<ResourceId>
    {
        public static readonly ResourceId Null = default;

        public ResourceId(string path)
        {
            Path = path;
            Id = null;
        }

        public ResourceId(string path, string id)
        {
            Path = path;
            Id = id;
        }

        public string Path { get; }

        public string Id { get; }

        public bool Equals(ResourceId other)
        {
            return string.Equals(Path, other.Path) && string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Path != null ? Path.GetHashCode() : 0) * 397) ^ (Id != null ? Id.GetHashCode() : 0);
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
            if (Id == null) return $"{Path}";
            return $"{Path}#{Id}";
        }
    }
}