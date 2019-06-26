using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace VeldridGlTF.Viewer.Components
{
    public class LocalTransform
    {
        private Flags _flags = Flags.Valid;
        private Matrix4x4 _matrix = Matrix4x4.Identity;
        private Vector3 _position = Vector3.Zero;
        private Quaternion _rotation = Quaternion.Identity;

        private Vector3 _scale = Vector3.One;
        public LocalTransform Parent;

        public Matrix4x4 Matrix
        {
            get
            {
                UpdateMatrixIfNeeded();
                return _matrix;
            }
            set
            {
                _flags |= Flags.InvalidPRS;
                _matrix = value;
            }
        }

        public Vector3 Scale
        {
            get
            {
                UpdatePRSIfNeeded();
                return _scale;
            }
            set
            {
                _flags |= Flags.InvalidMatrix;
                _scale = value;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                UpdatePRSIfNeeded();
                return _rotation;
            }
            set
            {
                _flags |= Flags.InvalidMatrix;
                _rotation = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                UpdatePRSIfNeeded();
                return _position;
            }
            set
            {
                _flags |= Flags.InvalidMatrix;
                _position = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMatrixIfNeeded()
        {
            if (Flags.InvalidMatrix == (_flags & Flags.InvalidMatrix))
            {
                _flags &= ~Flags.InvalidMatrix;
                _matrix = Matrix4x4.CreateScale(_scale) * Matrix4x4.CreateFromQuaternion(_rotation) *
                          Matrix4x4.CreateTranslation(_position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePRSIfNeeded()
        {
            if (Flags.InvalidPRS == (_flags & Flags.InvalidPRS))
            {
                _flags &= ~Flags.InvalidPRS;
                Matrix4x4.Decompose(_matrix, out _scale, out _rotation, out _position);
            }
        }

        public void EvaluateWorldTransform(out Matrix4x4 transform)
        {
            UpdateMatrixIfNeeded();
            if (Parent != null)
            {
                Parent.EvaluateWorldTransform(out var parent);
                transform = _matrix * parent;
            }
            else
            {
                transform = _matrix;
            }
        }

        [Flags]
        private enum Flags
        {
            Valid = 0,
            InvalidMatrix = 1,
            InvalidPRS = 2
        }
    }
}