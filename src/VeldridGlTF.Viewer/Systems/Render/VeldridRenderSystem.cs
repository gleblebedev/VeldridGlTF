﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
        private readonly Dictionary<PipelineKey, PipelineBinder> _pipelines = new Dictionary<PipelineKey, PipelineBinder>();

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
        private ImageSharpCubemapTexture _skyImage;
        private Texture _skyTexture;
        private TextureView _skyTextureView;

        #endregion

        private ImageSharpTexture _brdfLUTImage;
        private Texture _brdfLUTTexture;
        private TextureView _brdfLUTTextureView;

        private DeviceBuffer _viewBuffer;
        private DeviceBuffer _emptyUniform;

        private DeviceBuffer _worldBuffer;

        public VeldridRenderSystem(StepContext stepContext, IApplicationWindow window, bool enableRenderDoc)
        {
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
            _skyImage = LoadCubemapTexture(GetType().Assembly
                , "VeldridGlTF.Viewer.Assets.Sky.PosX.png"
                , "VeldridGlTF.Viewer.Assets.Sky.NegX.png"
                , "VeldridGlTF.Viewer.Assets.Sky.PosY.png"
                , "VeldridGlTF.Viewer.Assets.Sky.NegY.png"
                , "VeldridGlTF.Viewer.Assets.Sky.PosZ.png"
                , "VeldridGlTF.Viewer.Assets.Sky.NegZ.png"
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
            _camera.Yaw += deltaSeconds;
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

            //var lookAt = Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY);
            var lookAt = _camera.ViewMatrix;
            _cl.UpdateBuffer(_viewBuffer, 0, lookAt);
            var perspectiveFieldOfView = _camera.ProjectionMatrix;
            _cl.UpdateBuffer(_viewBuffer, 16*4, perspectiveFieldOfView);
            _cl.UpdateBuffer(_viewBuffer, 16 * 4+ 16 * 4, new []{_camera.Position});
            //_cl.UpdateBuffer(_projectionBuffer, 0, perspectiveFieldOfView);

            foreach (var modelIndex in _staticModels)
            {
                var worldTransform = _staticModels.Components1[modelIndex];
                var model = _staticModels.Components2[modelIndex];

                if (model.GetDrawCalls().TryGet(out var drawCallCollection) && drawCallCollection != null)
                    ScheduleDrawCalls(drawCallCollection, ref worldTransform.WorldMatrix);
            }

            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
            _graphicsDevice.WaitForIdle();
            _graphicsDevice.SwapBuffers(MainSwapchain);
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
            var identity = Matrix4x4.Identity;
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

        private void ScheduleDrawCalls(DrawCallCollection renderCache, ref Matrix4x4 worldMatrix)
        {
            _cl.SetIndexBuffer(renderCache.IndexBuffer, IndexFormat.UInt16);
            _cl.UpdateBuffer(_worldBuffer, 0, ref worldMatrix);
            for (var index = 0; index < renderCache.DrawCalls.Count; index++)
            {
                var drawCall = renderCache.DrawCalls[index];
                if (drawCall != null)
                {
                    var indexRange = renderCache.DrawCalls[index].Primitive;
                    var material = drawCall.Material;
                    if (material != null)
                    {
                        drawCall.Pipeline.Set(_cl, this, material);
                        _cl.SetVertexBuffer(0, renderCache.VertexBuffer, indexRange.DataOffset);
                        material.UpdateBuffer(_cl, MaterialBuffer);
                        _cl.DrawIndexed(indexRange.Length, 1, indexRange.Start, 0, 0);
                    }
                }
            }
        }

        public PipelineBinder GetPipeline(RenderPrimitive primitive, MaterialResource material, RenderPass pass)
        {
            var pipelineKey = EvaulatePipelineKey(primitive, material, pass);

            PipelineBinder pipelineBinder;

            if (_pipelines.TryGetValue(pipelineKey, out pipelineBinder)) return pipelineBinder;

            var shaderAndLayout = _shaderManager.GetShaders(pipelineKey.Shader, MainPass);
            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    primitive.Elements.VertexLayoutDescription
                },
                shaderAndLayout.Shaders);

            pipelineBinder = BuildResourceBinder(material.ResourceSetBuilder, shaderAndLayout.Layouts);

            pipelineBinder.Pipeline = _resourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
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
                pipelineBinder.ResourceLayouts,
                MainSwapchain.Framebuffer.OutputDescription));
            return pipelineBinder;
        }

        private PipelineBinder BuildResourceBinder(ResourceSetBuilder setBuilder, ResourceLayoutDescription[] layouts)
        {
            var pipelineBinder = new PipelineBinder();
            pipelineBinder.ResourceLayouts = new ResourceLayout[layouts.Length];
            pipelineBinder.Sets = new ResourceSet[layouts.Length];
            for (var index = 0; index < layouts.Length; index++)
            {
                BuildLayoutAndSet(setBuilder, layouts[index], out var layout, out var set);
                pipelineBinder.ResourceLayouts[index] = layout;
                if (layouts[index].Elements.Length > 0)
                {
                    pipelineBinder.Sets[index] = set;
                }
            }

            return pipelineBinder;
        }

        private void BuildLayoutAndSet(ResourceSetBuilder resourceSetBuilder, ResourceLayoutDescription layout, out ResourceLayout resLayout, out ResourceSet resSet)
        {
            resLayout = _resourceFactory.CreateResourceLayout(layout);
            resSet = _resourceFactory.CreateResourceSet(new ResourceSetDescription(resLayout, resourceSetBuilder.Resolve(_emptyUniform, layout.Elements)));
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
                PrimitiveTopology = primitive.PrimitiveTopology
            };
            pipelineKey.DepthStencilState = material.DepthStencilState;
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
            _viewBuffer =
                factory.CreateBuffer(new BufferDescription(64+64+16, BufferUsage.UniformBuffer));
            _worldBuffer =
                factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            MaterialBuffer = CreateMaterialBuffer();

            _brdfLUTTexture = _brdfLUTImage.CreateDeviceTexture(_graphicsDevice, _resourceFactory);
            _brdfLUTTextureView = factory.CreateTextureView(_brdfLUTTexture);

            _surfaceTexture = _albedoImage.CreateDeviceTexture(_graphicsDevice, _resourceFactory);
            //_surfaceTexture.Name = "DefaultDiffuseTexture";
            _defaultDiffuseTextureView = factory.CreateTextureView(_surfaceTexture);
            //_defaultDiffuseTextureView.Name = _surfaceTexture.Name;

            _skyTexture = _skyImage.CreateDeviceTexture(_graphicsDevice, _resourceFactory);
            //_skyTexture.Name = "DefaultSkyboxTexture";
            _skyTextureView = factory.CreateTextureView(_skyTexture);
            //_skyTextureView.Name = _surfaceTexture.Name;

            _resourceSetBuilder = new ResourceSetBuilder(
                _resourceFactory,
                new ResourceSetSlot("EnvironmentProperties", ResourceKind.UniformBuffer,  _viewBuffer),
                new ResourceSetSlot("BRDFTexture", ResourceKind.TextureReadOnly,  _brdfLUTTextureView), 
                new ResourceSetSlot("BRDFSampler", ResourceKind.Sampler,  _graphicsDevice.LinearSampler),
                new ResourceSetSlot("WorldBuffer", ResourceKind.UniformBuffer, _worldBuffer),
                new ResourceSetSlot(null, ResourceKind.UniformBuffer, _worldBuffer),
                new ResourceSetSlot("ReflectionTexture", ResourceKind.TextureReadOnly, _skyTextureView),
                new ResourceSetSlot("ReflectionSampler", ResourceKind.Sampler, _graphicsDevice.Aniso4xSampler)
                );

            _cl = factory.CreateCommandList();

            MainPass = new RenderPass("Main");

            _skybox = new Skybox {RenderSystem = this};
            var skyboxMaterial = new MaterialDescription(ResourceId.Null)
            {
                ShaderName = "Skybox",
                DiffuseTexture = new ManualResourceHandler<ITexture>(ResourceId.Null,
                    new TextureResource(ResourceId.Null, _skyTexture, _skyTextureView)),
                DepthWriteEnabled = false
            };
            var materialHandler = new ResourceHandler<IMaterial>(ResourceId.Null,
                _ => MaterialLoader.CreateMaterial(this, _, skyboxMaterial), null);
            _skybox.Material = materialHandler;
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