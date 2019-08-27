using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
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
                    metallicRoughnessParameters.AlphaCutoff = description.AlphaCutoff;

                    metallicRoughnessParameters = await SetBaseColorMap(context, metallicRoughness.BaseColor,
                        metallicRoughnessParameters, slots, renderContext);
                    metallicRoughnessParameters = await SetMetallicRoughnessMap(context,
                        metallicRoughness.MetallicRoughnessMap, metallicRoughnessParameters, slots, renderContext);
                    slots.Add(AddUniformBuffer(renderContext, disposables, ref metallicRoughnessParameters,
                        MaterialResource.Slots.MetallicRoughness));
                }

                var specularGlossiness = description.SpecularGlossiness;
                if (specularGlossiness != null)
                {
                    var specularGlossinessParameters = SpecularGlossiness.Identity;
                    specularGlossinessParameters.AlphaCutoff = description.AlphaCutoff;

                    specularGlossinessParameters = await SetDiffuseMap(context, specularGlossiness.Diffuse,
                        specularGlossinessParameters, slots, renderContext);
                    specularGlossinessParameters = await SetSpecularGlossinessMap(context,
                        specularGlossiness.SpecularGlossinessMap, specularGlossinessParameters, slots, renderContext);
                    slots.Add(AddUniformBuffer(renderContext, disposables, ref specularGlossinessParameters,
                        MaterialResource.Slots.SpecularGlossiness));
                }

                await SetEmissiveMap(context, description.Emissive, slots, renderContext, disposables);

                await SetNormalMap(context, description.Normal, slots, renderContext, disposables);

                await setOcclustionMap(context, description.Occlusion, slots, renderContext, disposables);

                material.DepthStencilState.DepthTestEnabled = description.DepthTestEnabled;
                material.DepthStencilState.DepthWriteEnabled = description.DepthWriteEnabled;
                material.AlphaMode = description.AlphaMode;

                material.ResourceSetBuilder = new ResourceSetBuilder(renderContext.Factory,
                    renderContext.RenderSystem.ResourceSetBuilder,
                    slots);
                material.Disposables = disposables;
                return material;
            }
            catch (Exception)
            {
                foreach (var disposable in disposables) disposable.Dispose();
                throw;
            }
        }

        private static async Task<MetallicRoughness> SetMetallicRoughnessMap(ResourceContext context,
            MapParameters metallicRoughnessMapParameters,
            MetallicRoughness metallicRoughnessParameters,
            List<ResourceSetSlot> slots, RenderContext renderContext)
        {
            if (metallicRoughnessMapParameters != null)
            {
                var metallicRoughnessMap =
                    await context.ResolveDependencyAsync(metallicRoughnessMapParameters.Map) as TextureResource;
                if (metallicRoughnessMap != null)
                {
                    slots.Add(new ResourceSetSlot(MaterialResource.Slots.MetallicRoughnessTexture,
                        ResourceKind.TextureReadOnly,
                        metallicRoughnessMap.View));
                    slots.Add(await CreateSamplerAsync(renderContext, MaterialResource.Slots.MetallicRoughnessSampler,
                        metallicRoughnessMapParameters));
                }

                metallicRoughnessParameters.MetallicRoughnessUVSet = metallicRoughnessMapParameters.UVSet;
                metallicRoughnessParameters.MetallicRoughnessUVTransform = metallicRoughnessMapParameters.UVTransform;
                metallicRoughnessParameters.MetallicFactor = metallicRoughnessMapParameters.Color.X;
                metallicRoughnessParameters.RoughnessFactor = metallicRoughnessMapParameters.Color.Y;
            }

            return metallicRoughnessParameters;
        }

        private static async Task<MetallicRoughness> SetBaseColorMap(ResourceContext context,
            MapParameters baseColorMapParameters,
            MetallicRoughness metallicRoughnessParameters,
            List<ResourceSetSlot> slots,
            RenderContext renderContext)
        {
            if (baseColorMapParameters != null)
            {
                var baseColor = await context.ResolveDependencyAsync(baseColorMapParameters.Map) as TextureResource;
                if (baseColor != null)
                {
                    slots.Add(new ResourceSetSlot(MaterialResource.Slots.BaseColorTexture, ResourceKind.TextureReadOnly,
                        baseColor.View));
                    slots.Add(await CreateSamplerAsync(renderContext, MaterialResource.Slots.BaseColorSampler,
                        baseColorMapParameters));
                }

                metallicRoughnessParameters.BaseColorUVSet = baseColorMapParameters.UVSet;
                metallicRoughnessParameters.BaseColorUVTransform = baseColorMapParameters.UVTransform;
                metallicRoughnessParameters.BaseColorFactor = baseColorMapParameters.Color;
            }

            return metallicRoughnessParameters;
        }

        private static async Task<SpecularGlossiness> SetSpecularGlossinessMap(ResourceContext context,
            MapParameters specularGlossinessMapParameters,
            SpecularGlossiness specularGlossinessParameters,
            List<ResourceSetSlot> slots, RenderContext renderContext)
        {
            if (specularGlossinessMapParameters != null)
            {
                var specularGlossinessMap =
                    await context.ResolveDependencyAsync(specularGlossinessMapParameters.Map) as TextureResource;
                if (specularGlossinessMap != null)
                {
                    slots.Add(new ResourceSetSlot(MaterialResource.Slots.SpecularGlossinessTexture,
                        ResourceKind.TextureReadOnly,
                        specularGlossinessMap.View));
                    slots.Add(await CreateSamplerAsync(renderContext, MaterialResource.Slots.SpecularGlossinessSampler,
                        specularGlossinessMapParameters));
                }

                specularGlossinessParameters.SpecularGlossinessUVSet = specularGlossinessMapParameters.UVSet;
                specularGlossinessParameters.SpecularGlossinessUVTransform =
                    specularGlossinessMapParameters.UVTransform;
                specularGlossinessParameters.SpecularFactor = new Vector3(specularGlossinessMapParameters.Color.X,
                    specularGlossinessMapParameters.Color.Y, specularGlossinessMapParameters.Color.Z);
                specularGlossinessParameters.GlossinessFactor = specularGlossinessMapParameters.Color.W;
            }

            return specularGlossinessParameters;
        }

        private static async Task<SpecularGlossiness> SetDiffuseMap(ResourceContext context,
            MapParameters diffuseMapParameters,
            SpecularGlossiness specularGlossinessParameters,
            List<ResourceSetSlot> slots,
            RenderContext renderContext)
        {
            if (diffuseMapParameters != null)
            {
                var diffuse = await context.ResolveDependencyAsync(diffuseMapParameters.Map) as TextureResource;
                if (diffuse != null)
                {
                    slots.Add(new ResourceSetSlot(MaterialResource.Slots.DiffuseTexture, ResourceKind.TextureReadOnly,
                        diffuse.View));
                    slots.Add(await CreateSamplerAsync(renderContext, MaterialResource.Slots.DiffuseSampler,
                        diffuseMapParameters));
                }

                specularGlossinessParameters.DiffuseUVSet = diffuseMapParameters.UVSet;
                specularGlossinessParameters.DiffuseUVTransform = diffuseMapParameters.UVTransform;
                specularGlossinessParameters.DiffuseFactor = diffuseMapParameters.Color;
            }

            return specularGlossinessParameters;
        }

        private static async Task SetEmissiveMap(ResourceContext context, MapParameters descriptionEmissive,
            List<ResourceSetSlot> slots,
            RenderContext renderContext, List<IDisposable> disposables)
        {
            if (descriptionEmissive != null)
            {
                var map = await context.ResolveDependencyAsync(descriptionEmissive.Map) as TextureResource;
                if (map != null)
                {
                    slots.Add(new ResourceSetSlot(MaterialResource.Slots.EmissiveTexture, ResourceKind.TextureReadOnly,
                        map.View));
                    slots.Add(await CreateSamplerAsync(renderContext, MaterialResource.Slots.EmissiveSampler,
                        descriptionEmissive));
                    var emissiveMapProperties = EmissiveMapProperties.Identity;
                    emissiveMapProperties.EmissiveUVSet = descriptionEmissive.UVSet;
                    emissiveMapProperties.EmissiveUVTransform = descriptionEmissive.UVTransform;
                    slots.Add(AddUniformBuffer(renderContext, disposables, ref emissiveMapProperties,
                        MaterialResource.Slots.EmissiveMapProperties));
                }
            }
        }

        private static async Task setOcclustionMap(ResourceContext context, MapParameters descriptionOcclusion,
            List<ResourceSetSlot> slots,
            RenderContext renderContext, List<IDisposable> disposables)
        {
            if (descriptionOcclusion != null)
            {
                var map = await context.ResolveDependencyAsync(descriptionOcclusion.Map) as TextureResource;
                if (map != null)
                {
                    slots.Add(new ResourceSetSlot(MaterialResource.Slots.OcclusionTexture, ResourceKind.TextureReadOnly,
                        map.View));
                    slots.Add(await CreateSamplerAsync(renderContext, MaterialResource.Slots.OcclusionSampler,
                        descriptionOcclusion));
                    var occlustionMapProperties = OcclusionMapProperties.Identity;
                    occlustionMapProperties.OcclusionUVSet = descriptionOcclusion.UVSet;
                    occlustionMapProperties.OcclusionUVTransform = descriptionOcclusion.UVTransform;
                    slots.Add(AddUniformBuffer(renderContext, disposables, ref occlustionMapProperties,
                        MaterialResource.Slots.OcclusionMapProperties));
                }
            }
        }

        private static async Task SetNormalMap(ResourceContext context, MapParameters descriptionNormal,
            List<ResourceSetSlot> slots,
            RenderContext renderContext, List<IDisposable> disposables)
        {
            if (descriptionNormal != null)
            {
                var map = await context.ResolveDependencyAsync(descriptionNormal.Map) as TextureResource;
                if (map != null)
                {
                    slots.Add(new ResourceSetSlot(MaterialResource.Slots.NormalTexture, ResourceKind.TextureReadOnly,
                        map.View));
                    slots.Add(await CreateSamplerAsync(renderContext, MaterialResource.Slots.NormalSampler,
                        descriptionNormal));
                    var normalMapProperties = NormalMapProperties.Identity;
                    normalMapProperties.NormalUVSet = descriptionNormal.UVSet;
                    normalMapProperties.NormalUVTransform = descriptionNormal.UVTransform;
                    slots.Add(AddUniformBuffer(renderContext, disposables, ref normalMapProperties,
                        MaterialResource.Slots.NormalMapProperties));
                }
            }
        }

        private static async Task<ResourceSetSlot> CreateSamplerAsync(RenderContext renderContext, string slot,
            MapParameters mapParameters)
        {
            var samplerDescription = SamplerDescription.Aniso4x;
            samplerDescription.AddressModeU = ConvertWrap(mapParameters.AddressModeU);
            samplerDescription.AddressModeV = ConvertWrap(mapParameters.AddressModeV);
            samplerDescription.AddressModeW = ConvertWrap(mapParameters.AddressModeW);
            var sampler = await renderContext.RenderSystem.GetOrCreateSampler(samplerDescription);
            return new ResourceSetSlot(slot, ResourceKind.Sampler, sampler);
        }

        private static SamplerAddressMode ConvertWrap(WrapMode occlusionAddressModeU)
        {
            return (SamplerAddressMode) occlusionAddressModeU;
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