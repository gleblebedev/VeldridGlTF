using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.Systems.Render;

namespace VeldridGlTF.Viewer.Data
{
    public class GlTFContainer
    {
        private readonly Dictionary<Node, EntityPrefab> _entities = new Dictionary<Node, EntityPrefab>();
        private readonly Dictionary<string, EntityPrefab> _entityById = new Dictionary<string, EntityPrefab>();

        public GlTFContainer(ResourceManager manager, string container, ModelRoot modelRoot)
        {
            {
                var index = 0;
                foreach (var texture in modelRoot.LogicalTextures)
                {
                    var id = string.IsNullOrWhiteSpace(texture.Name) ? "@" + index : texture.Name;
                    var resourceId = new ResourceId(container, id);
                    var handler = new ResourceHandler<ITexture>(resourceId,
                        () => Task.Run(() => (ITexture) new RenderTexture(resourceId, texture)));
                    manager.ResolveOrAdd(resourceId, handler);
                    Textures.Add(texture, id, handler);
                    ++index;
                }
            }

            {
                var index = 0;
                var m = new HashSet<Material>(modelRoot.LogicalMaterials);
                Debug.WriteLine("Unique materials: " + m.Count + " of " + modelRoot.LogicalMaterials.Count);
                foreach (var material in modelRoot.LogicalMaterials)
                {
                    var id = string.IsNullOrWhiteSpace(material.Name) ? "@" + index : material.Name;
                    while (Materials.ContainsId(id)) id += "_";
                    var resourceId = new ResourceId(container, id);
                    var result = new RenderMaterial(resourceId);
                    result.DiffuseColor = material.GetDiffuseColor(Vector4.One);
                    result.DiffuseTexture = Textures[material.GetDiffuseTexture()];
                    Func<Task<IMaterial>> factory = () => Task.FromResult((IMaterial) result);
                    var handler = new ResourceHandler<IMaterial>(resourceId, factory);
                    manager.ResolveOrAdd(resourceId, handler);
                    Materials.Add(material, id, handler);
                    ++index;
                }
            }
            {
                var index = 0;
                foreach (var mesh in modelRoot.LogicalMeshes)
                {
                    var id = string.IsNullOrWhiteSpace(mesh.Name) ? "@" + index : mesh.Name;
                    var resourceId = new ResourceId(container, id);
                    var handler =
                        new ResourceHandler<IMesh>(resourceId, () => Task.Run(() => (IMesh) new RenderMesh(mesh)));
                    manager.ResolveOrAdd(resourceId, handler);
                    Meshes.Add(mesh, id, handler);
                    ++index;
                }
            }
            {
                var index = 0;
                foreach (var node in modelRoot.LogicalNodes)
                {
                    var id = string.IsNullOrWhiteSpace(node.Name) ? "@" + index : node.Name;
                    var resourceId = new ResourceId(container, id);
                    var prefab = new EntityPrefab(resourceId);
                    prefab.LocalMatrix = node.LocalMatrix;
                    //var localTransform = node.LocalTransform;
                    //prefab.Position = localTransform.Translation;
                    //prefab.Rotation = localTransform.Rotation;
                    //prefab.Scale = localTransform.Scale;
                    //prefab.WorldMatrix = node.WorldMatrix;

                    //var t = new Transform(prefab.Position, prefab.Rotation, prefab.Scale);
                    //Matrix4x4 m;
                    //t.EvaluateMatrix(out m);

                    _entities.Add(node, prefab);
                    _entityById.Add(id, prefab);
                    Func<Task<EntityPrefab>> factory = () => Task.FromResult(prefab);
                    manager.ResolveOrAdd(resourceId, factory);

                    if (node.Mesh != null)
                    {
                        prefab.Mesh = Meshes[node.Mesh];

                        foreach (var meshPrimitive in node.Mesh.Primitives)
                            prefab.Materials.Add(Materials[meshPrimitive.Material]);
                    }

                    ++index;
                }

                foreach (var kv in _entities)
                foreach (var child in kv.Key.VisualChildren)
                    kv.Value.Children.Add(_entities[child]);

                foreach (var prefabKV in _entities)
                {
                    //if (prefabKV.Key.VisualParent != null)
                    //{
                    //    Matrix4x4 parentInv;
                    //    Matrix4x4.Invert(prefabKV.Key.VisualParent.WorldMatrix, out parentInv);
                    //    var localM = Matrix4x4.Multiply(prefabKV.Key.WorldMatrix, parentInv);

                    //    var prefab = prefabKV.Value;
                    //    Vector3 scale;
                    //    Quaternion rot;
                    //    Vector3 translation;
                    //    Matrix4x4.Decompose(localM, out scale, out rot, out translation);
                    //    prefab.Position = translation;
                    //    prefab.Rotation = rot;
                    //    prefab.Scale = scale;
                    //}
                }

                var rootElements = new List<EntityPrefab>();
                if (modelRoot.DefaultScene != null)
                    foreach (var child in modelRoot.DefaultScene.VisualChildren)
                        rootElements.Add(_entities[child]);

                if (rootElements.Count == 1)
                    Root = rootElements.First();
                else
                    Root = new EntityPrefab(new ResourceId(container, null), rootElements);
            }
        }

        public GlTFResourceCollection<Mesh, IResourceHandler<IMesh>> Meshes { get; } =
            new GlTFResourceCollection<Mesh, IResourceHandler<IMesh>>();

        public GlTFResourceCollection<Texture, IResourceHandler<ITexture>> Textures { get; } =
            new GlTFResourceCollection<Texture, IResourceHandler<ITexture>>();

        public GlTFResourceCollection<Material, IResourceHandler<IMaterial>> Materials { get; } =
            new GlTFResourceCollection<Material, IResourceHandler<IMaterial>>();

        public EntityPrefab Root { get; set; }

        private Vector4 GetColor(Material material)
        {
            if (material == null)
                return Vector4.One;
            var baseColor = material.FindChannel(KnownChannels.BaseColor.ToString());
            if (baseColor != null)
                return baseColor.Value.Parameter;
            var diffuse = material.FindChannel(KnownChannels.Diffuse.ToString());
            if (diffuse != null)
                return baseColor.Value.Parameter;
            return Vector4.One;
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