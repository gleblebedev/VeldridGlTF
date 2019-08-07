using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing.Dithering;
using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MaterialLoader : IResourceLoader<IMaterial>
    {
        private readonly VeldridRenderSystem _renderSystem;

        public MaterialLoader(VeldridRenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public async Task<IMaterial> LoadAsync(ResourceContext context)
        {
            var description = await context.ResolveDependencyAsync<IMaterialDescription>(context.Id);
            return await CreateMaterial(_renderSystem, context, description);
        }

        public static async Task<IMaterial> CreateMaterial(VeldridRenderSystem renderSystem, ResourceContext context,
            IMaterialDescription description)
        {
            var material = new MaterialResource(context.Id, renderSystem);
            material.ShaderName = description.ShaderName;

            var slots = new List<ResourceSetSlot>();
            var disposables = new List<IDisposable>();

            try
            {
                var renderContext = await context.ResolveDependencyAsync(renderSystem.RenderContext);

                material.Unlit = description.Unlit;

                var metallicRoughness = description.MetallicRoughness;
                if (metallicRoughness != null)
                {
                    var metallicRoughnessParameters = MetallicRoughness.Identity;

                    var baseColorMapParameters = metallicRoughness.BaseColor;
                    if (baseColorMapParameters != null)
                    {
                        var baseColor = await context.ResolveDependencyAsync(baseColorMapParameters.Map) as TextureResource;
                        if (baseColor != null)
                        {
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.BaseColorTexture, ResourceKind.TextureReadOnly,
                                baseColor.View));
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.BaseColorSampler, ResourceKind.Sampler,
                                renderContext.Device.Aniso4xSampler));
                        }

                        metallicRoughnessParameters.BaseColorFactor = baseColorMapParameters.Color;
                    }
                    var metallicRoughnessMapParameters = metallicRoughness.MetallicRoughnessMap;
                    if (metallicRoughnessMapParameters != null)
                    {
                        var metallicRoughnessMap = await context.ResolveDependencyAsync(metallicRoughnessMapParameters.Map) as TextureResource;
                        if (metallicRoughnessMap != null)
                        {
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.MetallicRoughnessTexture, ResourceKind.TextureReadOnly,
                                metallicRoughnessMap.View));
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.MetallicRoughnessSampler, ResourceKind.Sampler,
                                renderContext.Device.Aniso4xSampler));
                        }
                        metallicRoughnessParameters.MetallicFactor = metallicRoughnessMapParameters.Color.X;
                        metallicRoughnessParameters.RoughnessFactor = metallicRoughnessMapParameters.Color.Y;
                    }
                    slots.Add(AddUniformBuffer(renderContext, disposables, ref metallicRoughnessParameters, MaterialResource.Slots.MetallicRoughness));
                }

                var specularGlossiness = description.SpecularGlossiness;
                if (specularGlossiness != null)
                {
                    var specularGlossinessParameters = SpecularGlossiness.Identity;

                    var diffuseMapParameters = specularGlossiness.Diffuse;
                    if (diffuseMapParameters != null)
                    {
                        var diffuse = await context.ResolveDependencyAsync(diffuseMapParameters.Map) as TextureResource;
                        if (diffuse != null)
                        {
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.DiffuseTexture, ResourceKind.TextureReadOnly,
                                diffuse.View));
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.DiffuseSampler, ResourceKind.Sampler,
                                renderContext.Device.Aniso4xSampler));
                        }
                        specularGlossinessParameters.DiffuseFactor = diffuseMapParameters.Color;
                    }
                    var specularGlossinessMapParameters = specularGlossiness.SpecularGlossinessMap;
                    if (specularGlossinessMapParameters != null)
                    {
                        var specularGlossinessMap = await context.ResolveDependencyAsync(specularGlossinessMapParameters.Map) as TextureResource;
                        if (specularGlossinessMap != null)
                        {
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.SpecularGlossinessTexture, ResourceKind.TextureReadOnly,
                                specularGlossinessMap.View));
                            slots.Add(new ResourceSetSlot(MaterialResource.Slots.SpecularGlossinessSampler, ResourceKind.Sampler,
                                renderContext.Device.Aniso4xSampler));
                        }
                        specularGlossinessParameters.SpecularFactor = new Vector3(specularGlossinessMapParameters.Color.X, specularGlossinessMapParameters.Color.Y, specularGlossinessMapParameters.Color.Z);
                        specularGlossinessParameters.GlossinessFactor = specularGlossinessMapParameters.Color.W;
                    }
                    slots.Add(AddUniformBuffer(renderContext, disposables, ref specularGlossinessParameters, MaterialResource.Slots.SpecularGlossiness));
                }

                if (description.Emissive != null)
                {
                    var map = await context.ResolveDependencyAsync(description.Emissive.Map) as TextureResource;
                    if (map != null)
                    {
                        slots.Add(new ResourceSetSlot(MaterialResource.Slots.EmissiveTexture, ResourceKind.TextureReadOnly,
                            map.View));
                        slots.Add(new ResourceSetSlot(MaterialResource.Slots.EmissiveSampler, ResourceKind.Sampler,
                            renderContext.Device.Aniso4xSampler));
                        var emissiveMapProperties = EmissiveMapProperties.Identity;
                        slots.Add(AddUniformBuffer(renderContext, disposables, ref emissiveMapProperties, MaterialResource.Slots.EmissiveMapProperties));
                    }
                }

                if (description.Normal != null)
                {
                    var map = await context.ResolveDependencyAsync(description.Normal.Map) as TextureResource;
                    if (map != null)
                    {
                        slots.Add(new ResourceSetSlot(MaterialResource.Slots.NormalTexture, ResourceKind.TextureReadOnly,
                            map.View));
                        slots.Add(new ResourceSetSlot(MaterialResource.Slots.NormalSampler, ResourceKind.Sampler,
                            renderContext.Device.Aniso4xSampler));
                        var normalMapProperties = NormalMapProperties.Identity;
                        slots.Add(AddUniformBuffer(renderContext, disposables, ref normalMapProperties, MaterialResource.Slots.NormalMapProperties));
                    }
                }

                if (description.Occlusion != null)
                {
                    var map = await context.ResolveDependencyAsync(description.Occlusion.Map) as TextureResource;
                    if (map != null)
                    {
                        slots.Add(new ResourceSetSlot(MaterialResource.Slots.OcclusionTexture, ResourceKind.TextureReadOnly,
                            map.View));
                        slots.Add(new ResourceSetSlot(MaterialResource.Slots.OcclusionSampler, ResourceKind.Sampler,
                            renderContext.Device.Aniso4xSampler));
                    }
                }
                material.DepthStencilState.DepthTestEnabled = description.DepthTestEnabled;
                material.DepthStencilState.DepthWriteEnabled = description.DepthWriteEnabled;

                material.ResourceSetBuilder = new ResourceSetBuilder(renderContext.Factory,
                    renderContext.RenderSystem.ResourceSetBuilder,
                    slots);
                material.Disposables = disposables;
                return material;
            }
            catch (Exception)
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
                throw;
            }
        }

        private static ResourceSetSlot AddUniformBuffer<T>(RenderContext renderContext, List<IDisposable> disposables,
            ref T emissiveMapProperties, string name) where T : struct
        {
            var deviceBuffer = renderContext.Factory.CreateBuffer(
                new BufferDescription(VeldridRenderSystem.GetBufferSize<T>(), BufferUsage.UniformBuffer));
            disposables.Add(deviceBuffer);
            renderContext.Device.UpdateBuffer(deviceBuffer, 0, ref emissiveMapProperties);
            return new ResourceSetSlot(name, ResourceKind.UniformBuffer, deviceBuffer);
        }
    }
}