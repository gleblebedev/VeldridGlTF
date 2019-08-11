using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Leopotam.Ecs;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Loaders.GlTF;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.SceneGraph;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [EcsInject]
    public class VeldridRenderSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem, IRenderSystem
    {
        private readonly LazyAsyncCollection<PipelineKey, PipelineAndLayouts> _pipelines;

        private readonly ManualResourceHandler<RenderContext> _renderContext =
            new ManualResourceHandler<RenderContext>(ResourceId.Null);

        private readonly StepContext _stepContext;
        private readonly bool _enableRenderDoc;
        private ImageSharpTexture _albedoImage;
        private Texture _surfaceTexture;

        protected Camera _camera;
        private CommandList _cl;
        private TextureView _defaultDiffuseTextureView;
        private GraphicsDevice _graphicsDevice;
        //private DeviceBuffer _projectionBuffer;
        private RenderDoc _renderDoc;
        private ResourceSetBuilder _resourceSetBuilder;

        private ResourceFactory _resourceFactory;

        private ShaderManager _shaderManager;

        #region SKY

        private Skybox _skybox;

        private ImageSharpCubemapTexture _specularEnvImage;
        private Texture _specularEnvTexture;
        private TextureView _specularEnvTextureView;


        private ImageSharpCubemapTexture _diffuseEnvImage;
        private Texture _diffuseEnvTexture;
        private TextureView _diffuseEnvTextureView;

        #endregion

        private ImageSharpTexture _brdfLUTImage;
        private Texture _brdfLUTTexture;
        private TextureView _brdfLUTTextureView;

        private DeviceBuffer _environmentProperties;
        private DeviceBuffer _emptyUniform;

        private DeviceBuffer _objectProperties;

        public VeldridRenderSystem(StepContext stepContext, IApplicationWindow window, bool enableRenderDoc)
        {
            _pipelines = new LazyAsyncCollection<PipelineKey, PipelineAndLayouts>(CreatePipelineAsync);
            _stepContext = stepContext;
            _enableRenderDoc = enableRenderDoc;
            Window = window;
            Window.Resized += HandleWindowResize;
            Window.GraphicsDeviceCreated += OnGraphicsDeviceCreated;
            Window.GraphicsDeviceDestroyed += OnDeviceDestroyed;
            if (_enableRenderDoc)
            {
                RenderDoc.Load(out _renderDoc);
            }
        }

        public IResourceHandler<RenderContext> RenderContext => _renderContext;

        public Swapchain MainSwapchain { get; private set; }

        public IApplicationWindow Window { get; }

        public DeviceBuffer MaterialBuffer { get; private set; }

        public BindableResource DefaultTextureView => _defaultDiffuseTextureView;

        public void Initialize()
        {
            _camera = new Camera(Window.Width, Window.Height);
            _albedoImage = LoadTexture(GetType().Assembly, "VeldridGlTF.Viewer.Assets.Diffuse.png");
            //_specularEnvImage = LoadCubemapTexture(GetType().Assembly
            //    , "VeldridGlTF.Viewer.Assets.Sky.PosX.png"
            //    , "VeldridGlTF.Viewer.Assets.Sky.NegX.png"
            //    , "VeldridGlTF.Viewer.Assets.Sky.PosY.png"
            //    , "VeldridGlTF.Viewer.Assets.Sky.NegY.png"
            //    , "VeldridGlTF.Viewer.Assets.Sky.PosZ.png"
            //    , "VeldridGlTF.Viewer.Assets.Sky.NegZ.png"
            //);
            _specularEnvImage = LoadCubemapTexture(GetType().Assembly
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.specular.specular_right_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.specular.specular_left_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.specular.specular_top_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.specular.specular_bottom_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.specular.specular_front_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.specular.specular_back_0.jpg"
            );
            _diffuseEnvImage = LoadCubemapTexture(GetType().Assembly
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.diffuse.diffuse_right_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.diffuse.diffuse_left_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.diffuse.diffuse_top_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.diffuse.diffuse_bottom_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.diffuse.diffuse_front_0.jpg"
                , "VeldridGlTF.Viewer.Assets.Sky.papermill.diffuse.diffuse_back_0.jpg"
            );
            _brdfLUTImage = LoadTexture(GetType().Assembly, "VeldridGlTF.Viewer.Assets.brdfLUT.png");
        }

        public void Destroy()
        {
        }

        public void PreInitialize()
        {
        }

        public void PreDestroy()
        {
        }

        public void Run()
        {
            //float depthClear = GraphicsDevice.IsDepthRangeZeroToOne ? 0f : 1f;
            var deltaSeconds = _stepContext.DeltaSeconds;
            //_ticks += deltaSeconds * 1000f;

            _cl.Begin();

            _cl.SetFramebuffer(MainSwapchain.Framebuffer);
            //_cl.SetFullViewports();
            _cl.ClearDepthStencil(1f);

            RenderSkybox();


            //var perspectiveFieldOfView = Matrix4x4.CreatePerspectiveFieldOfView(
            //    1.0f,
            //    (float)Window.Width / Window.Height,
            //    0.5f,
            //    100f);

            var sceneBbox = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
            foreach (var modelIndex in _staticModels)
            {
                var worldTransform = _staticModels.Components1[modelIndex];
                var model = _staticModels.Components2[modelIndex];
                if (model.GetDrawCalls().TryGet(out var renderCache) && renderCache != null)
                {
                    var bbox = BoundingBox.Transform(renderCache.BoundingBox, worldTransform.WorldMatrix);
                    sceneBbox = BoundingBox.Combine(sceneBbox, bbox);
                }
            }

            _camera.Pitch = -0.5f;
            // Full turn = 7s, to match Gyazo GIF capture time.
            _camera.Yaw += 2.0f*(float)Math.PI*0.98f/7.0f*deltaSeconds;
            _camera.Position = _camera.Forward * -200;
            if (sceneBbox.Max.X > sceneBbox.Min.X)
            {
                var sceneCenter = sceneBbox.GetCenter();
                var sceneRadius = (sceneBbox.Max - sceneCenter).Length();
                if (sceneRadius < 1e-3f)
                    sceneRadius = 1e-3f;
                _camera.Position = sceneCenter + _camera.Forward * -sceneRadius * 2.0f;
                _camera.NearDistance = sceneRadius / 16.0f;
                _camera.FarDistance = sceneRadius * 4.0f;
            }

            UpdateEnvironment();

            var objectProperties = new ObjectProperties();
            foreach (var modelIndex in _staticModels)
            {
                var model = _staticModels.Components2[modelIndex];

                if (model.GetDrawCalls().TryGet(out var drawCallCollection) && drawCallCollection != null)
                {
                    var worldTransform = _staticModels.Components1[modelIndex];
                    objectProperties.ModelMatrix = worldTransform.WorldMatrix;
                    var n = worldTransform.WorldMatrix;
                    Matrix4x4 _n;
                    Matrix4x4.Invert(n, out _n);
                    n = Matrix4x4.Transpose(_n);
                    objectProperties.NormalMatrix = n;
                    unsafe
                    {
                        if (drawCallCollection.MorphWeights != null)
                        {
                            for (var index = 0; index < drawCallCollection.MorphWeights.Count; index++)
                            {
                                objectProperties.MorphWeights[index] = drawCallCollection.MorphWeights[index];
                            }
                        }
                    }

                    ScheduleDrawCalls(drawCallCollection, ref objectProperties);
                }
            }

            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
            _graphicsDevice.WaitForIdle();
            _graphicsDevice.SwapBuffers(MainSwapchain);
        }

        private void UpdateEnvironment()
        {
            var lookAt = _camera.ViewMatrix;
            var perspectiveFieldOfView = _camera.ProjectionMatrix;
            var data = new EnvironmentProperties();
            data.u_ViewProjectionMatrix = lookAt * perspectiveFieldOfView;
            data.u_Camera = _camera.Position;
            data.u_Exposure = 2.0f;
            data.u_MipCount = 10;
            _cl.UpdateBuffer(_environmentProperties, 0, data);
        }

        public IStaticModel AddStaticModel(EcsEntity entity)
        {
            var model = _world.AddComponent<StaticModel>(entity);
            model.RenderSystem = this;
            return model;
        }

        public ISkybox AddSkybox(EcsEntity entity)
        {
            var skybox = _world.AddComponent<Skybox>(entity);
            skybox.RenderSystem = this;
            return skybox;
        }

        public IZone AddZone(EcsEntity entity)
        {
            var zone = _world.AddComponent<Zone>(entity);
            zone.RenderSystem = this;
            return zone;
        }

        private void RenderSkybox()
        {
            var identity = new ObjectProperties();
            identity.ModelMatrix = Matrix4x4.Identity;
            identity.NormalMatrix = Matrix4x4.Identity;

            DrawCallCollection drawCallCollection;

            foreach (var index in _skyboxes)
            {
                var skybox = _skyboxes.Components2[index];
                if (skybox.GetDrawCalls().TryGet(out drawCallCollection) && drawCallCollection != null)
                {
                    ScheduleDrawCalls(drawCallCollection, ref identity);
                    return;
                }
            }

            if (_skybox.GetDrawCalls().TryGet(out drawCallCollection) && drawCallCollection != null)
            {
                ScheduleDrawCalls(drawCallCollection, ref identity);
                return;
            }

            _cl.ClearColorTarget(0, new RgbaFloat(48.0f / 255.0f, 10.0f / 255.0f, 36.0f / 255.0f, 1));
        }

        private void ScheduleDrawCalls(DrawCallCollection drawCallCollection, ref ObjectProperties objectProperties)
        {
            _cl.SetIndexBuffer(drawCallCollection.IndexBuffer, IndexFormat.UInt16);
            _cl.UpdateBuffer(_objectProperties, 0, ref objectProperties);
            for (var index = 0; index < drawCallCollection.DrawCalls.Count; index++)
            {
                var drawCall = drawCallCollection.DrawCalls[index];
                if (drawCall != null)
                {
                    var indexRange = drawCallCollection.DrawCalls[index].Primitive;
                    var material = drawCall.Material;
                    if (material != null)
                    {
                        drawCall.Pipeline.Set(_cl, this, material);
                        _cl.SetVertexBuffer(0, drawCallCollection.VertexBuffer, indexRange.DataOffset);
                        _cl.DrawIndexed(indexRange.Length, 1, indexRange.Start, 0, 0);
                    }
                }
            }
        }
        private async Task<PipelineAndLayouts> CreatePipelineAsync(PipelineKey pipelineKey)
        {
            var shaderAndLayout = await _shaderManager.GetShaders(pipelineKey.Shader);

            var shaderSet = new ShaderSetDescription(
                new []{pipelineKey.VertexLayout.VertexLayoutDescription},
                shaderAndLayout.Shaders);

            //BuildResourceBinder(material.ResourceSetBuilder, shaderAndLayout.Layouts,);

            var resourceLayouts = shaderAndLayout.Layouts.Select(_=>_resourceFactory.CreateResourceLayout(_)).ToArray();
            var graphicsPipelineDescription = new GraphicsPipelineDescription(
                GetBlendStateDescription(pipelineKey.AlphaMode),
                pipelineKey.DepthStencilState,
                new RasterizerStateDescription
                {
                    CullMode = FaceCullMode.Back,
                    FillMode = PolygonFillMode.Solid,
                    FrontFace = FrontFace.CounterClockwise,
                    DepthClipEnabled = true,
                    ScissorTestEnabled = false
                },
                pipelineKey.PrimitiveTopology,
                shaderSet,
                resourceLayouts,
                MainSwapchain.Framebuffer.OutputDescription);
            var graphicsPipeline = _resourceFactory.CreateGraphicsPipeline(graphicsPipelineDescription);

            return new PipelineAndLayouts()
            {
                Pipeline = graphicsPipeline, ResourceLayouts = resourceLayouts, Layouts = shaderAndLayout.Layouts, AlphaMode = pipelineKey.AlphaMode
            };
        }

        private BlendStateDescription GetBlendStateDescription(AlphaMode pipelineKeyAlphaMode)
        {
            switch (pipelineKeyAlphaMode)
            {
                case AlphaMode.Opaque:
                    return BlendStateDescription.SingleOverrideBlend;
                case AlphaMode.Mask:
                    return BlendStateDescription.SingleOverrideBlend;
                case AlphaMode.Blend:
                    return BlendStateDescription.SingleAlphaBlend;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pipelineKeyAlphaMode), pipelineKeyAlphaMode, null);
            }
        }

        public async Task<PipelineBinder> GetPipeline(RenderPrimitive primitive, MaterialResource material, RenderPass pass)
        {
            var pipelineKey = EvaulatePipelineKey(primitive, material, pass);
            var pipelineAndLayout = await _pipelines[pipelineKey];

            var sets = new ResourceSet[pipelineAndLayout.Layouts.Length];
            for (var index = 0; index < sets.Length; index++)
            {
                var resourceSetDescription = new ResourceSetDescription(pipelineAndLayout.ResourceLayouts[index], material.ResourceSetBuilder.Resolve(_emptyUniform, pipelineAndLayout.Layouts[index].Elements));
                sets[index] = _resourceFactory.CreateResourceSet(resourceSetDescription);
            }
            return new PipelineBinder()
            {
                Pipeline = pipelineAndLayout.Pipeline,
                ResourceLayouts = pipelineAndLayout.ResourceLayouts,
                Sets = sets
            };
        }


        public RenderPass MainPass { get; set; }

        public ResourceSetBuilder ResourceSetBuilder
        {
            get { return _resourceSetBuilder; }
        }

        private PipelineKey EvaulatePipelineKey(RenderPrimitive primitive, MaterialResource material, RenderPass renderPass)
        {
            var shaderKey = _shaderManager.GetShaderKey(primitive, material, renderPass);
            var pipelineKey = new PipelineKey
            {
                Shader = shaderKey,
                PrimitiveTopology = primitive.PrimitiveTopology,
                VertexLayout = primitive.Elements,
                DepthStencilState = material.DepthStencilState,
                AlphaMode = material.AlphaMode
            };
            return pipelineKey;
        }

        public void OnGraphicsDeviceCreated(GraphicsDevice gd, ResourceFactory factory, Swapchain sc)
        {
            _graphicsDevice = gd;
            _resourceFactory = factory;
            MainSwapchain = sc;
            CreateResources(factory);
            CreateSwapchainResources(factory);
            _renderContext.SetValue(new RenderContext(gd, factory, sc, this));
        }

        protected virtual void HandleWindowResize()
        {
            _camera.WindowResized(Window.Width, Window.Height);
        }

        protected virtual void CreateSwapchainResources(ResourceFactory factory)
        {
        }

        private ImageSharpTexture LoadTexture(Assembly assembly, string assetName)
        {
            using (var stream = assembly.GetManifestResourceStream(assetName))
            {
                if (stream != null)
                    return new ImageSharpTexture(stream);
            }

            return null;
        }

        private ImageSharpCubemapTexture LoadCubemapTexture(Assembly assembly, string posX, string negX, string posY,
            string negY, string posZ, string negZ)
        {
            var assets = new[] {posX, negX, posY, negY, posZ, negZ}
                .Select(_ => assembly.GetManifestResourceStream(_))
                .ToList();

            var cubemapTexture =
                new ImageSharpCubemapTexture(assets[0], assets[1], assets[2], assets[3], assets[4], assets[5], true);

            foreach (var stream in assets) stream.Dispose();

            return cubemapTexture;
        }

        protected void CreateResources(ResourceFactory factory)
        {
            _shaderManager = new ShaderManager(factory);
            _emptyUniform = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            //_projectionBuffer =
            //    factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _environmentProperties =
                factory.CreateBuffer(new BufferDescription(GetBufferSize<EnvironmentProperties>(), BufferUsage.UniformBuffer));
            _objectProperties =
                factory.CreateBuffer(new BufferDescription(GetBufferSize<ObjectProperties>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            MaterialBuffer = CreateMaterialBuffer();

            _brdfLUTTexture = _brdfLUTImage.CreateDeviceTexture(_graphicsDevice, _resourceFactory);
            _brdfLUTTextureView = factory.CreateTextureView(_brdfLUTTexture);

            _surfaceTexture = _albedoImage.CreateDeviceTexture(_graphicsDevice, _resourceFactory);
            //_surfaceTexture.Name = "DefaultDiffuseTexture";
            _defaultDiffuseTextureView = factory.CreateTextureView(_surfaceTexture);
            //_defaultDiffuseTextureView.Name = _surfaceTexture.Name;

            _specularEnvTexture = _specularEnvImage.CreateDeviceTexture(_graphicsDevice, _resourceFactory);
            //_skyTexture.Name = "DefaultSkyboxTexture";
            _specularEnvTextureView = factory.CreateTextureView(_specularEnvTexture);
            //_skyTextureView.Name = _surfaceTexture.Name;

            _diffuseEnvTexture = _diffuseEnvImage.CreateDeviceTexture(_graphicsDevice, _resourceFactory);
            //_skyTexture.Name = "DefaultSkyboxTexture";
            _diffuseEnvTextureView = factory.CreateTextureView(_diffuseEnvTexture);
            //_skyTextureView.Name = _surfaceTexture.Name;

            _resourceSetBuilder = new ResourceSetBuilder(
                _resourceFactory,
                new ResourceSetSlot("EnvironmentProperties", ResourceKind.UniformBuffer,  _environmentProperties),
                new ResourceSetSlot(MaterialResource.Slots.brdfLUTTexture, ResourceKind.TextureReadOnly,  _brdfLUTTextureView), 
                new ResourceSetSlot(MaterialResource.Slots.brdfLUTSampler, ResourceKind.Sampler,  _graphicsDevice.LinearSampler),
                new ResourceSetSlot("ObjectProperties", ResourceKind.UniformBuffer, _objectProperties),
                new ResourceSetSlot(null, ResourceKind.UniformBuffer, _objectProperties),
                new ResourceSetSlot(MaterialResource.Slots.DiffuseEnvTexture, ResourceKind.TextureReadOnly, _diffuseEnvTextureView),
                new ResourceSetSlot(MaterialResource.Slots.DiffuseEnvSampler, ResourceKind.Sampler, _graphicsDevice.Aniso4xSampler),
                new ResourceSetSlot(MaterialResource.Slots.SpecularEnvTexture, ResourceKind.TextureReadOnly, _specularEnvTextureView),
                new ResourceSetSlot(MaterialResource.Slots.SpecularEnvSampler, ResourceKind.Sampler, _graphicsDevice.Aniso4xSampler)
                );

            _cl = factory.CreateCommandList();

            MainPass = new RenderPass("Main");

            _skybox = new Skybox {RenderSystem = this};
            var skyboxMaterial = new MaterialDescription(ResourceId.Null)
            {
                ShaderName = "Skybox",
                SpecularGlossiness = new Data.SpecularGlossiness()
                {
                    Diffuse = new MapParameters()
                    {
                        Map = new ManualResourceHandler<ITexture>(ResourceId.Null,
                            new TextureResource(ResourceId.Null, _specularEnvTexture, _specularEnvTextureView))
                    }
                },
                DepthWriteEnabled = false
            };
            var materialHandler = new ResourceHandler<IMaterial>(ResourceId.Null,
                _ => MaterialLoader.CreateMaterial(this, _, skyboxMaterial), null);
            _skybox.Material = materialHandler;
        }

        public static uint GetBufferSize<T>() where T: struct
        {
            var sizeOf = Marshal.SizeOf<T>();
            if (0 != (sizeOf & 15))
            {
                sizeOf = (sizeOf & ~15) + 16;
            }
            return (uint)sizeOf;
        }

        protected virtual void OnDeviceDestroyed()
        {
            if (_graphicsDevice != null)
                _graphicsDevice = null;
            else
                _renderContext.SetException(new Exception("Device wasn't created."));

            if (_resourceFactory != null)
                _resourceFactory = null;

            MainSwapchain = null;
            _renderContext.Reset();
        }

        protected virtual string GetTitle()
        {
            return GetType().Name;
        }

        public V ResolveHandler<T, V>(IResourceHandler<T> handler, V defaultValue = default) where V : class
        {
            if (handler == null)
                return defaultValue;
            if (handler.Status != TaskStatus.RanToCompletion)
                return defaultValue;
            return handler.GetAsync().Result as V;
        }

        public DeviceBuffer CreateMaterialBuffer()
        {
            var materialBuffer =
                _resourceFactory.CreateBuffer(
                    new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            return materialBuffer;
        }

        internal StaticModel CreateStaticModel()
        {
            return new StaticModel {RenderSystem = this};
        }

        #region ECS

        private EcsWorld _world = null;
        private EcsFilter<WorldTransform, Skybox> _skyboxes = null;
        private EcsFilter<WorldTransform, StaticModel> _staticModels = null;
        private EcsFilter<WorldTransform, Zone> _zones = null;

        #endregion
    }
}