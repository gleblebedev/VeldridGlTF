using System.Collections.Generic;
using System.Numerics;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public interface IGeometryStream
    {
        string Key { get; }
        GeometryStreamFormat Format { get; }
        void Write(int index, VertexBufferStream vbStream);
        IReadOnlyList<Vector4> AsVector4();
        IReadOnlyList<Vector3> AsVector3();
        IReadOnlyList<Vector2> AsVector2();
        IReadOnlyList<float> AsScalar();
    }
}