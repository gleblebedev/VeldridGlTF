using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Leopotam.Ecs;
using Veldrid;
using Veldrid.ImageSharp;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.SceneGraph;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [EcsInject]
    public class VeldridRenderSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilter<WorldTransform, Model> _staticModels = null;
        private readonly StepContext _stepContext;

        protected Camera _camera;
        private CommandList _cl;
        private DeviceBuffer _materialBuffer;
        private Dictionary<PipelineKey, Pipeline> _pipelines = new Dictionary<PipelineKey, Pipeline>();
        private DeviceBuffer _projectionBuffer;
        private ResourceSet _environmentSet;
        private ResourceSet _meshSet;
        private ResourceSet _defaultMaterialSet;
        private ShaderManager _shaderManager;
        private ImageSharpTexture _defaultTexture;
        private Texture _surfaceTexture;
        private TextureView _defaultDiffuseTextureView;
        private float _ticks;

        private DeviceBuffer _viewBuffer;

        private EcsWorld _world = null;
        private DeviceBuffer _worldBuffer;
        private ResourceLayout _environmentLayout;
        private ResourceLayout _meshLayout;
        private ResourceLayout _materialLayout;

        public VeldridRenderSystem(StepContext stepContext, IApplicationWindow window)
        {
            _stepContext = stepContext;
            Window = window;
            Window.Resized += HandleWindowResize;
            Window.GraphicsDeviceCreated += OnGraphicsDeviceCreated;
            Window.GraphicsDeviceDestroyed += OnDeviceDestroyed;
        }

        public GraphicsDevice GraphicsDevice { get; private set; }
        public ResourceFactory ResourceFactory { get; private set; }
        public Swapchain MainSwapchain { get; private set; }

        public IApplicationWindow Window { get; }

        public void Initialize()
        {
            _camera = new Camera(Window.Width, Window.Height);
            _defaultTexture = LoadTexture(GetType().Assembly, "VeldridGlTF.Viewer.Assets.Avocado_baseColor.png");
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
            _ticks += deltaSeconds * 1000f;

            _camera.Pitch = -0.5f;
            _camera.Yaw += deltaSeconds;
            _camera.Position = _camera.Forward * -200;

            _cl.Begin();

            _cl.SetFramebuffer(MainSwapchain.Framebuffer);
            //_cl.SetFullViewports();
            _cl.ClearColorTarget(0, new RgbaFloat(48.0f / 255.0f, 10.0f / 255.0f, 36.0f / 255.0f, 1));
            _cl.ClearDepthStencil(1f);

            //var perspectiveFieldOfView = Matrix4x4.CreatePerspectiveFieldOfView(
            //    1.0f,
            //    (float)Window.Width / Window.Height,
            //    0.5f,
            //    100f);
            var perspectiveFieldOfView = _camera.ProjectionMatrix;
            _cl.UpdateBuffer(_projectionBuffer, 0, perspectiveFieldOfView);

            //var lookAt = Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY);
            var lookAt = _camera.ViewMatrix;
            _cl.UpdateBuffer(_viewBuffer, 0, lookAt);

            foreach (var modelIndex in _staticModels)
            {
                var worldTransform = _staticModels.Components1[modelIndex];
                var model = _staticModels.Components2[modelIndex];
                if (model.RenderContext as RenderContext == null)
                    model.RenderContext = new RenderContext(this, model);
                var context = (RenderContext)model.RenderContext;
                context.Update();
                var renderMesh = ResolveHandler<IMesh, RenderMesh>(model.Mesh);
                if (renderMesh != null)
                {
                    renderMesh.EnsureResources(GraphicsDevice, ResourceFactory);

                    _cl.UpdateBuffer(_worldBuffer, 0, ref worldTransform.WorldMatrix);

                    _cl.SetIndexBuffer(renderMesh._indexBuffer, IndexFormat.UInt16);
                    for (var index = 0; index < context.DrawCalls.Count; index++)
                    {
                        var drawCall = context.DrawCalls[index];
                        if (drawCall != null)
                        {
                            var indexRange = renderMesh.Primitives[index];
                            var material = drawCall.Material;
                            if (material != null)
                            {
                                material.EnsureResources(this);
                                _cl.SetPipeline(drawCall.Pipeline);
                                _cl.SetGraphicsResourceSet(0, _environmentSet);
                                _cl.SetGraphicsResourceSet(1, _meshSet);
                                _cl.SetGraphicsResourceSet(2, material.MaterialSet ?? _defaultMaterialSet);
                                _cl.SetVertexBuffer(0, renderMesh._vertexBuffer, indexRange.DataOffset);
                                _cl.UpdateBuffer(_materialBuffer, 0, ref material.DiffuseColor);
                                _cl.DrawIndexed(indexRange.Length, 1, indexRange.Start, 0, 0);
                            }
                        }
                    }
                }
            }

            _cl.End();
            GraphicsDevice.SubmitCommands(_cl);
            GraphicsDevice.WaitForIdle();
            GraphicsDevice.SwapBuffers(MainSwapchain);
        }

        public Pipeline GetPipeline(PipelineKey pipelineKey)
        {
            Pipeline pipeline;
            if (_pipelines.TryGetValue(pipelineKey, out pipeline))
            {
                return pipeline;
            }

            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    pipelineKey.Shader.VertexLayout.VertexLayoutDescription
                },
                _shaderManager.GetShaders(pipelineKey.Shader));

            pipeline = ResourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
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
                new[] { _environmentLayout, _meshLayout, _materialLayout },
                MainSwapchain.Framebuffer.OutputDescription));
            return pipeline;
        }

        public void OnGraphicsDeviceCreated(GraphicsDevice gd, ResourceFactory factory, Swapchain sc)
        {
            GraphicsDevice = gd;
            ResourceFactory = factory;
            MainSwapchain = sc;
            CreateResources(factory);
            CreateSwapchainResources(factory);
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


        protected void CreateResources(ResourceFactory factory)
        {
            _shaderManager = new ShaderManager(factory);
            _projectionBuffer =
                factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _viewBuffer =
                factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _worldBuffer =
                factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _materialBuffer =
                factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _surfaceTexture = _defaultTexture.CreateDeviceTexture(GraphicsDevice, ResourceFactory);
            _defaultDiffuseTextureView = factory.CreateTextureView(_surfaceTexture);

            _environmentLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex)));

            _meshLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex, ResourceLayoutElementOptions.None)
                ));

            _materialLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly,
                        ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler,
                        ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("MaterialProperties", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)
                ));

            _environmentSet = factory.CreateResourceSet(new ResourceSetDescription(
                _environmentLayout,
                _projectionBuffer,
                _viewBuffer));

            _meshSet = factory.CreateResourceSet(new ResourceSetDescription(
                _meshLayout,
                _worldBuffer
            ));

            _defaultMaterialSet = factory.CreateResourceSet(new ResourceSetDescription(
                _materialLayout,
                _defaultDiffuseTextureView,
                GraphicsDevice.Aniso4xSampler,
                _materialBuffer
            ));

            _cl = factory.CreateCommandList();
        }

        protected virtual void OnDeviceDestroyed()
        {
            GraphicsDevice = null;
            ResourceFactory = null;
            MainSwapchain = null;
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

        public ResourceSet CreateMaterialSet(RenderMaterial material)
        {
            var diffuseTexture = _defaultDiffuseTextureView;
            if (material.DiffuseTexture != null)
            {
                var texture = material.DiffuseTexture.GetAsync().Result as RenderTexture;
                if (texture != null)
                {
                    texture.EnsureResources(this);
                    diffuseTexture = texture.TextureView ?? diffuseTexture;
                }
            }

            return ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                _materialLayout,
                diffuseTexture,
                GraphicsDevice.Aniso4xSampler,
                _materialBuffer
            ));
        }
    }
}