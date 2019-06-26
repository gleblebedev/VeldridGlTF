using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform
    {
        public static readonly Transform Identity = new Transform() {Position = Vector3.Zero, Rotation = Quaternion.Identity, Scale = Vector3.One};

        public Vector3 Position;
        private readonly float _padding0;
        public Quaternion Rotation;
        public Vector3 Scale;
        private readonly float _padding1;

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            _padding0 = 0;
            Rotation = rotation;
            _padding1 = 0;
            Scale = scale;
        }

        public Transform(Vector3 position, Quaternion rotation):this(position, rotation, Vector3.One)
        {
        }
        public Transform(Quaternion rotation) : this(Vector3.Zero, rotation, Vector3.One)
        {
        }

        public Transform(Vector3 position) : this(position, Quaternion.Identity, Vector3.One)
        {
        }

        public static Transform operator* (Transform a,Transform b)
        {
            Matrix4x4 am, bm;
            a.EvaluateMatrix(out am);
            b.EvaluateMatrix(out bm);
            var cm = am * bm;
            var result = new Transform();
            Matrix4x4.Decompose(cm, out result.Position, out result.Rotation, out result.Scale);
            return result;
        }

        public void EvaluateMatrix(out Matrix4x4 matrix)
        {
            matrix = Matrix4x4.CreateScale(Scale)
                     * 
                     Matrix4x4.CreateFromQuaternion(Rotation) 
                     *
                     Matrix4x4.CreateTranslation(Position);
        }
    }
}