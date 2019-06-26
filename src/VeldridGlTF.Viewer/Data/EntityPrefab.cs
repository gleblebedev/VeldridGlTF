using System.Collections.Generic;
using System.Numerics;
using Leopotam.Ecs;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Data
{
    public class EntityPrefab
    {
        private readonly ResourceId _id;
        List<EntityPrefab> _children = new List<EntityPrefab>();

        public IList<EntityPrefab> Children
        {
            get { return _children; }
        }

        public EntityPrefab(ResourceId id)
        {
            _id = id;
        }
        public EntityPrefab(ResourceId id, IEnumerable<EntityPrefab> children):this(id)
        {
            _children.AddRange(children);
        }

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;

        public IResourceHandler<IMesh> Mesh { get; set; }

        public ResourceId Id
        {
            get { return _id; }
        }

        public EcsEntity Spawn(EcsWorld world, LocalTransform parent = null)
        {
            var entity = world.CreateEntity();
            var lt = world.AddComponent<LocalTransform>(entity);
            lt.Transform = new Transform(Position,Rotation,Scale);
            var wt = world.AddComponent<WorldTransform>(entity);
            if (Mesh != null)
            {
                var staticModel = world.AddComponent<StaticModel>(entity);
                staticModel.Model = Mesh;
            }

            foreach (var child in _children)
            {
                child.Spawn(world, lt);
            }
            return entity;
        }
    }
}