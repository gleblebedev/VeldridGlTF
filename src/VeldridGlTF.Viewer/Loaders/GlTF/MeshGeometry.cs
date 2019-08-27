using System.Collections.Generic;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Data.Geometry;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class MeshGeometry : AbstractResource, IGeometry
    {
        private readonly List<IGeometryPrimitive> _primitives;

        public MeshGeometry(ResourceId id, Mesh mesh, Skin skin) : base(id)
        {
            _primitives = new List<IGeometryPrimitive>(mesh.Primitives.Count);
            foreach (var meshPrimitive in mesh.Primitives) _primitives.Add(new Primitive(meshPrimitive));
            MorphWeights = mesh.MorphWeights;
            if (skin != null)
                JointCount = (uint)skin.JointsCount;
        }

        public IReadOnlyList<float> MorphWeights { get; }
        public uint JointCount { get; }

        public IReadOnlyList<IGeometryPrimitive> Primitives => _primitives;
    }
}