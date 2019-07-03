using System;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public struct ShaderKey : IEquatable<ShaderKey>
    {
        public bool Equals(ShaderKey other)
        {
            return string.Equals(ShaderName, other.ShaderName) && Equals(VertexLayout, other.VertexLayout) &&
                   Flags == other.Flags;
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ShaderName != null ? ShaderName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (VertexLayout != null ? VertexLayout.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Flags;
                return hashCode;
            }
        }

        public static bool operator ==(ShaderKey left, ShaderKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderKey left, ShaderKey right)
        {
            return !left.Equals(right);
        }

        public string ShaderName { get; set; }

        public RenderVertexLayout VertexLayout { get; set; }

        public ShaderFlag Flags { get; set; }

        public bool IsFlagSet(ShaderFlag flag)
        {
            return flag == (Flags & flag);
        }
    }
}