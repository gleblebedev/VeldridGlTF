﻿using System.Collections.Generic;
using System.Numerics;
using Leopotam.Ecs;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public class EntityPrefab
    {
        private readonly List<EntityPrefab> _children = new List<EntityPrefab>();

        public Matrix4x4? LocalMatrix;
        public Vector3? Position;
        public Quaternion? Rotation;
        public Vector3? Scale;

        public EntityPrefab(ResourceId id)
        {
            Id = id;
        }

        public EntityPrefab(ResourceId id, IEnumerable<EntityPrefab> children) : this(id)
        {
            _children.AddRange(children);
        }

        public IList<EntityPrefab> Children => _children;

        public IResourceHandler<IMesh> Mesh { get; set; }
        public IList<IResourceHandler<IMaterial>> Materials { get; } = new List<IResourceHandler<IMaterial>>();

        public ResourceId Id { get; }

        public SceneGraph.Node Spawn(SceneGraph.Scene scene, SceneGraph.Node parent = null)
        {
            var node = new SceneGraph.Node(scene);
            node.Parent = parent;

            if (LocalMatrix.HasValue) node.Transform.Matrix = LocalMatrix.Value;

            if (Position.HasValue) node.Transform.Position = Position.Value;

            if (Rotation.HasValue) node.Transform.Rotation = Rotation.Value;

            if (Scale.HasValue) node.Transform.Scale = Scale.Value;

            if (Mesh != null)
            {
                var staticModel = node.AddComponent<StaticModel>();
                staticModel.Model = Mesh;
                staticModel.Materials.Clear();
                foreach (var resourceHandler in Materials)
                {
                    staticModel.Materials.Add(resourceHandler);
                }
            }

            foreach (var child in _children) child.Spawn(scene, node);
            return node;
        }
    }
}