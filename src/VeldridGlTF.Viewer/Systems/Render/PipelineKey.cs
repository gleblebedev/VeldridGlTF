using System;
using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.Shaders;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class PipelineKey : IEquatable<PipelineKey>
    {
        public bool Equals(PipelineKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DepthStencilState.Equals(other.DepthStencilState) && PrimitiveTopology == other.PrimitiveTopology && Equals(Shader, other.Shader) && Equals(VertexLayout, other.VertexLayout);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PipelineKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DepthStencilState.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) PrimitiveTopology;
                hashCode = (hashCode * 397) ^ (Shader != null ? Shader.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (VertexLayout != null ? VertexLayout.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PipelineKey left, PipelineKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PipelineKey left, PipelineKey right)
        {
            return !Equals(left, right);
        }

        public DepthStencilStateDescription DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;
        public PrimitiveTopology PrimitiveTopology = PrimitiveTopology.TriangleList;
        public ShaderKey Shader;
        public RenderVertexLayout VertexLayout;
    }
}