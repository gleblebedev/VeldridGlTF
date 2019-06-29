using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public class Node : NodeContainer
    {
        private readonly Scene _scene;

        private readonly EcsEntity _entity;
        private Node _parent;
        private readonly LocalTransform _transform;

        private WorldMatrixToken _worldMatrixVersion = WorldMatrixToken.Empty;
        private readonly WorldTransform _worldTransform;
        private WorldMatrixToken _worldTransformToken;

        public Node(Scene scene)
        {
            _scene = scene;
            _entity = _scene.World.CreateEntityWith(out _transform, out _worldTransform);
            _transform.Parent = null;
            _transform.OnUpdate += HandleTransformUpdate;
            _transform.Reset();
            Add(this, scene);
        }

        public LocalTransform Transform => _transform;

        public Node Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    if (value != null && value._scene != _scene)
                        throw new InvalidOperationException(
                            "Can't move Node to a different scene. Please create a new node in the scene.");
                    if (_parent != null)
                        Remove(this, _parent);
                    else
                        Remove(this, _scene);
                    _parent = value;
                    if (_parent != null)
                    {
                        _transform.Parent = _parent._transform;
                        Add(this, _parent);
                    }
                    else
                    {
                        Add(this, _scene);
                        _transform.Parent = null;
                    }

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
            if (_worldTransformToken == WorldMatrixToken.Empty)
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

        private void UpdateSubtreeWorldTransform(WorldMatrixToken updateToken)
        {
            EnsureWorldTransformIsUpToDate(updateToken);
            var q = new Queue<Node>();
            foreach (var child in Children) q.Enqueue(child);

            while (q.Count > 0)
            {
                var n = q.Dequeue();
                if (n._worldTransformToken != WorldMatrixToken.Empty)
                {
                    if (n._worldMatrixVersion != updateToken)
                    {
                        n._worldTransform.WorldMatrix = n._transform.Matrix * n._parent._worldTransform.WorldMatrix;
                        n._worldMatrixVersion = updateToken;
                    }

                    foreach (var child in n.Children) q.Enqueue(child);
                }
            }
        }

        private void EnsureWorldTransformIsUpToDate(WorldMatrixToken updateToken)
        {
            if (_worldMatrixVersion != updateToken)
            {
                _worldMatrixVersion = updateToken;
                if (_parent == null)
                {
                    _worldTransform.WorldMatrix = _transform.Matrix;
                }
                else
                {
                    _parent.EnsureWorldTransformIsUpToDate(updateToken);
                    _worldTransform.WorldMatrix = _transform.Matrix * _parent._worldTransform.WorldMatrix;
                }
            }
        }

        public class WorldMatrixUpdateQueue
        {
            private readonly List<Node> _queue = new List<Node>(128);
            private int _updateCounter;

            public void Update()
            {
                while (_queue.Count != 0)
                {
                    ++_updateCounter;
                    var updateToken = new WorldMatrixToken(_updateCounter);
                    foreach (var node in _queue) node.UpdateSubtreeWorldTransform(updateToken);
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