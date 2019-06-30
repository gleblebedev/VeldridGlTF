using System;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public struct WorldMatrixToken : IEquatable<WorldMatrixToken>
    {
        public bool Equals(WorldMatrixToken other)
        {
            return _index == other._index;
        }

        public override bool Equals(object obj)
        {
            return obj is WorldMatrixToken other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _index;
        }

        public static bool operator ==(WorldMatrixToken left, WorldMatrixToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WorldMatrixToken left, WorldMatrixToken right)
        {
            return !left.Equals(right);
        }

        private readonly int _index;

        public WorldMatrixToken(int index)
        {
            _index = index;
        }

        public static readonly WorldMatrixToken Empty = new WorldMatrixToken(0);

        public override string ToString()
        {
            return (_index == 0)?"<Empty>":string.Format("<{0}>",_index);
        }
    }
}