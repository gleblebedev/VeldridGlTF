using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data
{
    public class GlTFContainer
    {
        public GlTFResourceCollection<Mesh, IResourceHandler<IMesh>> Meshes { get;  } = new GlTFResourceCollection<Mesh, IResourceHandler<IMesh>>();
        private Dictionary<string, EntityPrefab> _entityById = new Dictionary<string, EntityPrefab>();
        private Dictionary<Node, EntityPrefab> _entities = new Dictionary<Node, EntityPrefab>();
        private EntityPrefab _root;

        public GlTFContainer(ResourceManager manager, string container, ModelRoot modelRoot)
        {
            {
                int index = 0;
                foreach (var mesh in modelRoot.LogicalMeshes)
                {
                    var id = string.IsNullOrWhiteSpace(mesh.Name) ? "@" + index : mesh.Name;
                    var resourceId = new ResourceId(container, id);
                    var handler = new ResourceHandler<IMesh>(resourceId,  () => Task.Run(() => (IMesh)new RenderMesh(mesh)));
                    manager.ResolveOrAdd(resourceId, handler);
                    Meshes.Add(mesh, id, handler);
                    ++index;
                }
            }
            {
                int index = 0;
                foreach (var node in modelRoot.LogicalNodes)
                {
                    var id = string.IsNullOrWhiteSpace(node.Name) ? "@" + index : node.Name;
                    var resourceId = new ResourceId(container, id);
                    var prefab = new EntityPrefab(resourceId) { Position = node.LocalTransform.Translation, Rotation = node.LocalTransform.Rotation, Scale = node.LocalTransform.Scale };
                    _entities.Add(node, prefab);
                    _entityById.Add(id, prefab);
                    Func<Task<EntityPrefab>> factory = () => Task.FromResult(prefab);
                    manager.ResolveOrAdd(resourceId, factory);

                    if (node.Mesh != null)
                    {
                        prefab.Mesh = Meshes[node.Mesh];
                    }
                    ++index;
                }
                foreach (var kv in _entities)
                {
                    foreach (var child in kv.Key.VisualChildren)
                    {
                        kv.Value.Children.Add(_entities[child]);
                    }
                }

                var rootElements = new List<EntityPrefab>();
                if (modelRoot.DefaultScene != null)
                {
                    foreach (var child in modelRoot.DefaultScene.VisualChildren)
                    {
                        rootElements.Add(_entities[child]);
                    }
                }

                if (rootElements.Count == 1)
                {
                    Root = rootElements[0];
                }
                else
                {
                    Root = new EntityPrefab(new ResourceId(container, null), rootElements);
                }
            }
        }

        public EntityPrefab Root
        {
            get { return _root; }
            set { _root = value; }
        }

        public EntityPrefab GetEntity(string id)
        {
            EntityPrefab res;
            if (!_entityById.TryGetValue(id, out res))
                return null;
            return res;
        }
    }
}