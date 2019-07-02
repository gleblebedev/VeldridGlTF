using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class RenderVertexLayout : IEquatable<RenderVertexLayout>
    {
        private readonly string _name;

        public RenderVertexLayout(IEnumerable<VertexElementDescription> elements)
        {
            var descriptions = elements.ToArray();
            VertexLayoutDescription = new VertexLayoutDescription(descriptions);
            _name = string.Join(",", descriptions.Select(_ => _.Format + " " + _.Name));
        }

        public VertexLayoutDescription VertexLayoutDescription { get; }

        public bool Equals(RenderVertexLayout other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RenderVertexLayout) obj);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public static bool operator ==(RenderVertexLayout left, RenderVertexLayout right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RenderVertexLayout left, RenderVertexLayout right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}