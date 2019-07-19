using System;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders
{
    public class ShaderKey : IEquatable<ShaderKey>
    {
        private readonly RenderPass _renderPass;

        public ShaderKey(IShaderFactory factory, RenderPass renderPass)
        {
            _renderPass = renderPass;
            Factory = factory;
        }

        public IShaderFactory Factory { get; }

        public RenderPass RenderPass
        {
            get { return _renderPass; }
        }

        public virtual bool Equals(ShaderKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            if (other.Factory != Factory) return false;
            if (other._renderPass != _renderPass) return false;
            return true;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ShaderKey) obj);
        }

        public static bool operator ==(ShaderKey left, ShaderKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ShaderKey left, ShaderKey right)
        {
            return !Equals(left, right);
        }
    }
}