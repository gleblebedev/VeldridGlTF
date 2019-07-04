using System;
using System.Collections.Generic;
using System.Numerics;
using Leopotam.Ecs;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public class Node : NodeContainer
    {
        private readonly EcsEntity _entity;
        private readonly NodeComponent _nodeComponent;
        private readonly Scene _scene;
        private readonly WorldTransform _worldTransform;
        private Node _parent;

        private WorldMatrixToken _worldMatrixVersion = WorldMatrixToken.Empty;
        private WorldMatrixToken _worldTransformToken;

        public Node(Scene scene, bool hasTransform = true)
        {
            //TODO: Make Node constructor internal
            _scene = scene;
            _entity = _scene.World.CreateEntityWith(out _nodeComponent);
            _nodeComponent.Node = this;
            if (hasTransform)
            {
                Transform = _scene.World.AddComponent<LocalTransform>(_entity);
                Transform.OnUpdate += HandleTransformUpdate;
                Transform.Reset();

                _worldTransform = _scene.World.AddComponent<WorldTransform>(_entity);
                _worldTransform.WorldMatrix = Matrix4x4.Identity;
            }

            Add(this, scene);
        }

        public LocalTransform Transform { get; }

        public Node Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    if (value != null)
                    {
                        if (value._scene != _scene)
                            throw new InvalidOperationException(
                                "Can't move Node to a different scene. Please create a new node in the scene.");

                        if (Transform != null && value.Transform == null)
                            throw new InvalidOperationException(
                                "Can't attach node with transform to a parent node with no transform");
                    }

                    if (_parent != null)
                        Remove(this, _parent);
                    else
                        Remove(this, _scene);

                    _parent = value;
                    if (_parent != null)
                        Add(this, _parent);
                    else
                        Add(this, _scene);

                    InvalidateWorldTransform();
                }
            }
        }

        private void HandleTransformUpdate(object sender, TransformUpdatedArgs e)
        {
            InvalidateWorldTransform();
        }

        private void InvalidateWorldTransform()
        {
            if (Transform != null && _worldTransformToken == WorldMatrixToken.Empty)
                // Don't schedule an update if parent is already scheduled.
                // This could save time on prefab spawn and animations.
                if (_parent == null || _parent._worldTransformToken == WorldMatrixToken.Empty)
                    _worldTransformToken = _scene.EnqueueWorldTransformUpdate(this);
        }

        public T GetComponent<T>() where T : class, new()
        {
            return _scene.World.GetComponent<T>(_entity);
        }

        public T AddComponent<T>() where T : class, new()
        {
            return _scene.World.AddComponent<T>(_entity);
        }

        private void ResetToken()
        {
            _worldTransformToken = WorldMatrixToken.Empty;
        }

        private void UpdateSubtreeWorldTransform(WorldMatrixToken updateToken, Queue<Node> updateQueue)
        {
            EnsureWorldTransformIsUpToDate(updateToken);
            if (HasChildren)
                foreach (var child in Children)
                    updateQueue.Enqueue(child);

            while (updateQueue.Count > 0)
            {
                var n = updateQueue.Dequeue();
                if (n.Transform != null && n._worldTransformToken == WorldMatrixToken.Empty)
                {
                    if (n._worldMatrixVersion != updateToken)
                    {
                        n._worldTransform.WorldMatrix = n.Transform.Matrix * n._parent._worldTransform.WorldMatrix;
                        n._worldMatrixVersion = updateToken;
                    }

                    if (n.HasChildren)
                        foreach (var child in n.Children)
                            updateQueue.Enqueue(child);
                }
            }
        }

        public void EvaluateWorldTransform(out Matrix4x4 m)
        {
            if (_parent != null)
            {
                _parent.EvaluateWorldTransform(out var parent);
                if (Transform != null)
                {
                    m = Transform.Matrix * parent;
                    return;
                }

                m = parent;
                return;
            }

            if (Transform != null)
            {
                m = Transform.Matrix;
                return;
            }

            m = Matrix4x4.Identity;
        }

        private void EnsureWorldTransformIsUpToDate(WorldMatrixToken updateToken)
        {
            if (_worldMatrixVersion != updateToken)
            {
                _worldMatrixVersion = updateToken;
                if (_parent == null)
                {
                    _worldTransform.WorldMatrix = Transform.Matrix;
                }
                else
                {
                    _parent.EnsureWorldTransformIsUpToDate(updateToken);
                    _worldTransform.WorldMatrix = Transform.Matrix * _parent._worldTransform.WorldMatrix;
                }
            }
        }

        public class WorldMatrixUpdateQueue
        {
            private readonly List<Node> _queue = new List<Node>(128);
            private readonly Queue<Node> _updateQueue = new Queue<Node>(128);
            private int _updateCounter;

            public void Update()
            {
                while (_queue.Count != 0)
                {
                    ++_updateCounter;
                    var updateToken = new WorldMatrixToken(_updateCounter);
                    foreach (var node in _queue) node.UpdateSubtreeWorldTransform(updateToken, _updateQueue);
                    foreach (var node in _queue) node.ResetToken();
                    _queue.Clear();
                }
            }

            public WorldMatrixToken Add(Node node)
            {
                _queue.Add(node);
                return new WorldMatrixToken(_queue.Count);
            }
        }
    }
}