using System.Collections.Generic;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data
{
    public interface IGeometry
    {
        IReadOnlyList<IGeometryPrimitive> Primitives { get; }
    }

    public interface IGeometryPrimitive
    {
        IReadOnlyList<IGeometryStream> Streams { get; }
        PrimitiveTopology Topology { get; }
        IReadOnlyCollection<int> Indices { get; }
    }

    public interface IGeometryStream
    {
        string Key { get; }
        GeometryStreamFormat Format { get; }
        void Write(int index, VertexBufferStream vbStream);
    }
}