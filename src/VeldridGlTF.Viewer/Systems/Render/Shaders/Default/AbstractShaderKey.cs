using System;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Default
{
    public abstract class AbstractShaderKey : IEquatable<AbstractShaderKey>
    {
        public abstract bool Equals(AbstractShaderKey other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AbstractShaderKey) obj);
        }

        public static bool operator ==(AbstractShaderKey left, AbstractShaderKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractShaderKey left, AbstractShaderKey right)
        {
            return !Equals(left, right);
        }
    }
}