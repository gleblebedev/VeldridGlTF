using System;
using System.Collections.Generic;
using Veldrid;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render.Resources
{
    public class MaterialResource : AbstractResource, IMaterial, IDisposable
    {
        private readonly VeldridRenderSystem _renderSystem;
        private ResourceSetBuilder _resourceSet;
        public AlphaMode AlphaMode = AlphaMode.Opaque;

        public DepthStencilStateDescription DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;


        public MaterialResource(ResourceId id, VeldridRenderSystem renderSystem) : base(id)
        {
            _renderSystem = renderSystem;
        }

        public ResourceSetBuilder ResourceSetBuilder
        {
            get => _resourceSet;
            internal set
            {
                if (_resourceSet != null) _resourceSet.Dispose();
                _resourceSet = value;
            }
        }

        public string ShaderName { get; set; } = "Default";

        public List<IDisposable> Disposables { get; set; }

        public bool Unlit { get; set; }

        public override void Dispose()
        {
            ResourceSetBuilder?.Dispose();
            foreach (var disposable in Disposables) disposable?.Dispose();
            base.Dispose();
        }

        public static class Slots
        {
            public const string SpecularGlossiness = "SpecularGlossiness";
            public const string DiffuseTexture = "DiffuseTexture";
            public const string DiffuseSampler = "DiffuseSampler";
            public const string SpecularGlossinessTexture = "SpecularGlossinessTexture";
            public const string SpecularGlossinessSampler = "SpecularGlossinessSampler";

            public const string MetallicRoughness = "MetallicRoughness";
            public const string MetallicRoughnessTexture = "MetallicRoughnessTexture";
            public const string MetallicRoughnessSampler = "MetallicRoughnessSampler";
            public const string BaseColorTexture = "BaseColorTexture";
            public const string BaseColorSampler = "BaseColorSampler";

            public const string NormalTexture = "NormalTexture";
            public const string NormalSampler = "NormalSampler";
            public const string NormalMapProperties = "NormalMapProperties";

            public const string EmissiveTexture = "EmissiveTexture";
            public const string EmissiveSampler = "EmissiveSampler";
            public const string EmissiveMapProperties = "EmissiveMapProperties";

            public const string OcclusionTexture = "OcclusionTexture";
            public const string OcclusionSampler = "OcclusionSampler";
            public const string OcclusionMapProperties = "OcclusionMapProperties";

            public const string brdfLUTTexture = "brdfLUTTexture";
            public const string brdfLUTSampler = "brdfLUTSampler";

            public const string DiffuseEnvTexture = "DiffuseEnvTexture";
            public const string DiffuseEnvSampler = "DiffuseEnvSampler";

            public const string SpecularEnvTexture = "SpecularEnvTexture";
            public const string SpecularEnvSampler = "SpecularEnvSampler";
        }
    }
}