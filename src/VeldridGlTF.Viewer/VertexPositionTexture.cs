using System.Numerics;

namespace VeldridGlTF.Viewer
{
    public struct VertexPositionTexture
    {
        public const uint SizeInBytes = (3+2+3)*4;

        public Vector3 Pos;

        public Vector2 UV;

        public Vector3 Normal;


        public VertexPositionTexture(Vector3 pos, Vector2 uv, Vector3 n)
        {
            Pos = pos;
            UV = uv;
            Normal = n;
        }
    }
}