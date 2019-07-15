using System.Collections.Generic;
using VeldridGlTF.Viewer.Data.Geometry;

namespace VeldridGlTF.Viewer.Data
{
    public interface IGeometry
    {
        IReadOnlyList<IGeometryPrimitive> Primitives { get; }
    }
}