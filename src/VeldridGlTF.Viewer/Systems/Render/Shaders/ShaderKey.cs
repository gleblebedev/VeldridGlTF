using System;
using Veldrid.SPIRV;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders
{
    public class ShaderKey : IEquatable<ShaderKey>
    {
        public ShaderKey(IShaderFactory factory, ILayoutNameResolver layoutNameResolver = null)
        {
            Factory = factory;
            LayoutNameResolver = layoutNameResolver;
        }

        public IShaderFactory Factory { get; }

        public ILayoutNameResolver LayoutNameResolver { get; }

        public virtual bool Equals(ShaderKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Factory, other.Factory) && Equals(LayoutNameResolver, other.LayoutNameResolver);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ShaderKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Factory != null ? Factory.GetHashCode() : 0) * 397) ^
                       (LayoutNameResolver != null ? LayoutNameResolver.GetHashCode() : 0);
            }
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