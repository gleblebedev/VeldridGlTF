using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;
using AlphaMode = VeldridGlTF.Viewer.Data.AlphaMode;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class GlTFContainer : IResourceContainer
    {
        //private readonly Dictionary<Node, EntityPrefab> _entities = new Dictionary<Node, EntityPrefab>();
        //private readonly Dictionary<string, EntityPrefab> _entityById = new Dictionary<string, EntityPrefab>();
        public GlTFResourceCollection<Node, IResourceHandler<EntityPrefab>> Entities { get; } =
            new GlTFResourceCollection<Node, IResourceHandler<EntityPrefab>>();

        public GlTFResourceCollection<Mesh, IResourceHandler<IGeometry>> Meshes { get; } =
            new GlTFResourceCollection<Mesh, IResourceHandler<IGeometry>>();

        public GlTFResourceCollection<Texture, IResourceHandler<IImage>> Textures { get; } =
            new GlTFResourceCollection<Texture, IResourceHandler<IImage>>();

        public GlTFResourceCollection<Material, IResourceHandler<IMaterialDescription>> Materials { get; } =
            new GlTFResourceCollection<Material, IResourceHandler<IMaterialDescription>>();

        public EntityPrefab Root { get; set; }

        public IResourceHandler<T> Resolve<T>(ResourceId id)
        {
            if (typeof(T) == typeof(IImage)) return (IResourceHandler<T>) Textures[id.Id];
            if (typeof(T) == typeof(EntityPrefab)) return (IResourceHandler<T>) Entities[id.Id];
            if (typeof(T) == typeof(IMaterialDescription)) return (IResourceHandler<T>) Materials[id.Id];
            if (typeof(T) == typeof(IGeometry)) return (IResourceHandler<T>) Meshes[id.Id];

            throw new NotImplementedException("Resource type " + typeof(T) + " isn't supported by container");
        }

        public async Task ParseFile(ResourceContext context)
        {
            var file = await context.ResolveDependencyAsync<IFile>(context.Id);
            ModelRoot modelRoot;
            using (var stream = file.Open())
            {
                if (stream.CanSeek)
                {
                    modelRoot = ModelRoot.Read(stream,
                        new ReadSettings {FileReader = new FileReader(context).ReadAsset});
                }
                else
                {
                    var buf = new MemoryStream();
                    stream.CopyTo(buf);
                    buf.Position = 0;
                    modelRoot = ModelRoot.Read(buf,
                        new ReadSettings {FileReader = new FileReader(context).ReadAsset});
                }
            }


            var container = context.Id.Path;

            {
                RegisterTextures(modelRoot, container);
            }

            {
                RegisterAnimations(modelRoot, container);
            }

            {
                RegisterMaterials(context, modelRoot, container);
            }
            {
                RegisterMeshes(modelRoot, container);
            }
            {
                var index = 0;
                var prefabs = new Dictionary<Node, EntityPrefab>();
                foreach (var node in modelRoot.LogicalNodes)
                {
                    var id = string.IsNullOrWhiteSpace(node.Name) ? "@" + index : node.Name;
                    while (Entities.ContainsId(id)) id += "_";
                    var resourceId = new ResourceId(container, id);
                    var prefab = new EntityPrefab(resourceId);
                    prefab.LocalMatrix = node.LocalMatrix;
                    Entities.Add(node, id, new ManualResourceHandler<EntityPrefab>(resourceId, prefab));
                    prefabs.Add(node, prefab);

                    if (node.Mesh != null)
                    {
                        prefab.Mesh = context.Resolve<IMesh>(Meshes[node.Mesh].Id);

                        foreach (var meshPrimitive in node.Mesh.Primitives)
                            prefab.Materials.Add(context.Resolve<IMaterial>(Materials[meshPrimitive.Material].Id));
                    }
                    ++index;
                }

                foreach (var kv in Entities)
                foreach (var child in kv.Key.VisualChildren)
                    kv.Value.GetAsync().Result.Children.Add(prefabs[child]);


                var rootElements = new List<EntityPrefab>();
                if (modelRoot.DefaultScene != null)
                    foreach (var child in modelRoot.DefaultScene.VisualChildren)
                        rootElements.Add(prefabs[child]);

                if (rootElements.Count == 1)
                    Root = rootElements.First();
                else
                    Root = new EntityPrefab(new ResourceId(container, null), rootElements);

                Entities.Add(null, null, new ManualResourceHandler<EntityPrefab>(new ResourceId(container), Root));
            }
        }

        private void RegisterMeshes(ModelRoot modelRoot, string container)
        {
            var index = 0;
            foreach (var mesh in modelRoot.LogicalMeshes)
            {
                var id = string.IsNullOrWhiteSpace(mesh.Name) ? "@" + index : mesh.Name;
                var resourceId = new ResourceId(container, id);
                Meshes.Add(mesh, id,
                    new ManualResourceHandler<IGeometry>(resourceId, new MeshGeometry(resourceId, mesh)));
                ++index;
            }
        }

        private void RegisterMaterials(ResourceContext context, ModelRoot modelRoot, string container)
        {
            var index = 0;
            var m = new HashSet<Material>(modelRoot.LogicalMaterials);
            Debug.WriteLine("Unique materials: " + m.Count + " of " + modelRoot.LogicalMaterials.Count);
            foreach (var material in modelRoot.LogicalMaterials)
            {
                var id = string.IsNullOrWhiteSpace(material.Name) ? "@" + index : material.Name;
                while (Materials.ContainsId(id)) id += "_";
                var resourceId = new ResourceId(container, id);

                //result.BaseColor = material.GetDiffuseColor(Vector4.One);
                //var resourceHandler = Textures[material.GetDiffuseTexture()];
                //if (resourceHandler != null) result.DiffuseTexture = context.Resolve<ITexture>(resourceHandler.Id);

                Materials.Add(material, id, new ManualResourceHandler<IMaterialDescription>(resourceId, MakeMaterialDescription(resourceId, material, context)));
                ++index;
            }
        }

        private MaterialDescription MakeMaterialDescription(ResourceId resourceId, Material material,
            ResourceContext context)
        {
            var result = new MaterialDescription(resourceId);
            {
                switch (material.Alpha)
                {
                    case SharpGLTF.Schema2.AlphaMode.OPAQUE:
                        result.AlphaMode = AlphaMode.Opaque;
                        break;
                    case SharpGLTF.Schema2.AlphaMode.MASK:
                        result.AlphaMode = AlphaMode.Mask;
                        break;
                    case SharpGLTF.Schema2.AlphaMode.BLEND:
                        result.AlphaMode = AlphaMode.Blend;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                result.AlphaCutoff = material.AlphaCutoff;
            }

            {
                result.Unlit = material.Unlit;
            }

            {
                var materialMap = ResolveTexture(material, context, "BaseColor");
                if (materialMap != null)
                {
                    var specularGlossiness = result.MetallicRoughness ?? (result.MetallicRoughness = new MetallicRoughness());
                    specularGlossiness.BaseColor = materialMap;
                }
            }
            {
                var materialMap = ResolveTexture(material, context, "MetallicRoughness");
                if (materialMap != null)
                {
                    var specularGlossiness = result.MetallicRoughness ?? (result.MetallicRoughness = new MetallicRoughness());
                    specularGlossiness.MetallicRoughnessMap = materialMap;
                }
            }
            {
                result.Normal = ResolveTexture(material, context, "Normal");
            }
            {
                result.Occlusion = ResolveTexture(material, context, "Occlusion");
            }
            {
                result.Emissive = ResolveTexture(material, context, "Emissive");
            }
            return result;
        }

        private MapParameters ResolveTexture(Material material, ResourceContext context, string channelKey)
        {
            var baseColor = material.FindChannel(channelKey);
            if (baseColor != null)
            {
                if (baseColor.Value.Texture != null)
                {
                    var resourceHandler = Textures[baseColor.Value.Texture];
                    if (resourceHandler != null)
                    {
                        return new MapParameters()
                        {
                            Map = context.Resolve<ITexture>(resourceHandler.Id)
                        };
                    }
                }
            }

            return null;
        }

        private void RegisterTextures(ModelRoot modelRoot, string container)
        {
            var index = 0;
            foreach (var texture in modelRoot.LogicalTextures)
            {
                var id = string.IsNullOrWhiteSpace(texture.Name) ? "@" + index : texture.Name;
                var resourceId = new ResourceId(container, id);
                Textures.Add(texture, id,
                    new ManualResourceHandler<IImage>(resourceId, new EmbeddedImage(resourceId, texture)));
                ++index;
            }
        }

        private static void RegisterAnimations(ModelRoot modelRoot, string container)
        {
            var index = 0;
            foreach (var animnation in modelRoot.LogicalAnimations)
            {
                var id = string.IsNullOrWhiteSpace(animnation.Name) ? "@" + index : animnation.Name;
                var resourceId = new ResourceId(container, id);
                ++index;
            }
        }

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
    }
}