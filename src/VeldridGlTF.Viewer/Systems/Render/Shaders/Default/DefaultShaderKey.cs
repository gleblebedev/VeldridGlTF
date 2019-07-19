using System;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Default
{
    public class DefaultShaderKey : ShaderKey, IEquatable<DefaultShaderKey>
    {
        public DefaultShaderKey(IShaderFactory factory, RenderPass renderPass, RenderVertexLayout layout) : base(factory, renderPass)
        {
            VertexLayout = layout;
            foreach (var element in VertexLayout.VertexLayoutDescription.Elements)
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

        public RenderVertexLayout VertexLayout { get; }

        public ShaderFlag Flags { get; set; }

        public bool Equals(DefaultShaderKey other)
        {
            if (!base.Equals((ShaderKey) other))
                return false;
            return Equals(VertexLayout, other.VertexLayout) && Flags == other.Flags;
        }

        public override bool Equals(ShaderKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((DefaultShaderKey) other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DefaultShaderKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((VertexLayout != null ? VertexLayout.GetHashCode() : 0) * 397) ^ (int) Flags;
            }
        }

        public static bool operator ==(DefaultShaderKey left, DefaultShaderKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DefaultShaderKey left, DefaultShaderKey right)
        {
            return !Equals(left, right);
        }

        public bool IsFlagSet(ShaderFlag flag)
        {
            return flag == (Flags & flag);
        }
    }
}