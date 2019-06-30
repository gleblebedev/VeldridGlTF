using System;
using System.Collections.Generic;
using System.Threading;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public abstract class NodeContainer : INodeContainer
    {
        private Lazy<HashSet<Node>> _children = new Lazy<HashSet<Node>>(LazyThreadSafetyMode.PublicationOnly);

        public IReadOnlyCollection<Node> Children => _children.Value;

        public bool HasChildren
        {
            get { return _children.IsValueCreated && _children.Value.Count != 0; }
        }

        protected void Add(Node node, NodeContainer container)
        {
            container._children.Value.Add(node);
        }

        protected void Remove(Node node, NodeContainer container)
        {
            container._children.Value.Remove(node);
        }
    }
}