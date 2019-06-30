using System.Collections.Generic;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public interface INodeContainer
    {
        IReadOnlyCollection<Node> Children { get; }

        bool HasChildren { get; }
    }
}