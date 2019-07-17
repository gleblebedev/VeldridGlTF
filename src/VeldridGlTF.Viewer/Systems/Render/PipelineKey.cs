using System;
using Veldrid;
using VeldridGlTF.Viewer.Systems.Render.Shaders;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class PipelineKey : IEquatable<PipelineKey>
    {
        public DepthStencilStateDescription DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;
        public PrimitiveTopology PrimitiveTopology = PrimitiveTopology.TriangleList;
        public ShaderKey Shader;

        public bool Equals(PipelineKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PrimitiveTopology == other.PrimitiveTopology && Shader.Equals(other.Shader);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PipelineKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) PrimitiveTopology * 397) ^ Shader.GetHashCode();
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
    }
}