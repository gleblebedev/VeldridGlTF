using System;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders
{
    public class ShaderKey : IEquatable<ShaderKey>
    {
        public ShaderKey(IShaderFactory factory)
        {
            Factory = factory;
        }

        public IShaderFactory Factory { get; }

        public virtual bool Equals(ShaderKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            if (other.Factory != Factory) return false;
            return Equals(other);
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