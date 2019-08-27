using System.Collections.Generic;

namespace VeldridGlTF.Viewer.Data.Geometry
{
    public interface IGeometryPrimitive
    {
        IReadOnlyList<IGeometryStream> Streams { get; }
        PrimitiveTopology Topology { get; }
        IReadOnlyCollection<int> Indices { get; }
        bool HasSkin { get; }
    }
}