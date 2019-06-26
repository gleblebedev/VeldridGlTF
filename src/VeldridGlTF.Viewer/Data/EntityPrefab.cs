using System.Collections.Generic;
using System.Numerics;
using Leopotam.Ecs;
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
        public IResourceHandler<IMaterial> Material { get; set; }

        public ResourceId Id { get; }

        public EcsEntity Spawn(EcsWorld world, LocalTransform parent = null)
        {
            var entity = world.CreateEntity();
            var lt = world.AddComponent<LocalTransform>(entity);
            if (LocalMatrix.HasValue) lt.Matrix = LocalMatrix.Value;

            if (Position.HasValue) lt.Position = Position.Value;

            if (Rotation.HasValue) lt.Rotation = Rotation.Value;

            if (Scale.HasValue) lt.Scale = Scale.Value;

            lt.Parent = parent;
            var wt = world.AddComponent<WorldTransform>(entity);
            if (Mesh != null)
            {
                var staticModel = world.AddComponent<StaticModel>(entity);
                staticModel.Model = Mesh;
                staticModel.Material = Material;
            }

            foreach (var child in _children) child.Spawn(world, lt);
            return entity;
        }
    }
}