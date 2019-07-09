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

        public void SetLayout(RenderVertexLayout layout)
        {
            VertexLayout = layout;
            foreach (var element in VertexLayout.VertexLayoutDescription.Elements)
            {
                switch (element.Name)
                {
                    case "NORMAL":
                        Flags |= ShaderFlag.HAS_NORMALS;
                        break;
                    case "TANGENT":
                        Flags |= ShaderFlag.HAS_TANGENTS;
                        break;
                    case "TEXCOORD_0":
                        Flags |= ShaderFlag.HAS_UV_SET1;
                        break;
                    case "TEXCOORD_1":
                        Flags |= ShaderFlag.HAS_UV_SET2;
                        break;
                    case "COLOR_0":
                        Flags |= ShaderFlag.HAS_VERTEX_COLOR;
                        break;
                }
            }
        }
    }
}