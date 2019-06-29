﻿using System.Collections.Generic;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public abstract class NodeContainer: INodeContainer
    {
        private HashSet<Node> _children = new HashSet<Node>();
        public IReadOnlyCollection<Node> Children
        {
            get { return _children; }
        }

        protected void Add(Node node, NodeContainer container)
        {
            container._children.Add(node);
        }

        protected void Remove(Node node, NodeContainer container)
        {
            container._children.Remove(node);
        }

    }
}