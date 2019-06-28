using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Leopotam.Ecs;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Components;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [EcsInject]
    public class VeldridRenderSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem
    {
        private readonly StepContext _stepContext;

        protected Camera _camera;
        private CommandList _cl;
        private DeviceBuffer _materialBuffer;
        private Pipeline _pipeline;
        private DeviceBuffer _projectionBuffer;
        private ResourceSet _projViewSet;
        private readonly EcsFilter<WorldTransform, StaticModel> _staticModels = null;
        private ImageSharpTexture _stoneTexData;
        private Texture _surfaceTexture;
        private TextureView _surfaceTextureView;
        private float _ticks;

        private DeviceBuffer _viewBuffer;

        private EcsWorld _world = null;
        private DeviceBuffer _worldBuffer;
        private ResourceSet _worldTextureSet;

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
            _stoneTexData = LoadTexture(GetType().Assembly, "VeldridGlTF.Viewer.Assets.Avocado_baseColor.png");
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
            var deltaSeconds = _stepContext.DeltaSeconds;
            _ticks += deltaSeconds * 1000f;

            _camera.Pitch = -0.5f;
            _camera.Yaw += deltaSeconds;
            _camera.Position = _camera.Forward * -200;

            _cl.Begin();

            _cl.SetFramebuffer(MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, RgbaFloat.Black);
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
            _cl.SetPipeline(_pipeline);

            foreach (var modelIndex in _staticModels)
            {
                var worldTransform = _staticModels.Components1[modelIndex];
                var staticModel = _staticModels.Components2[modelIndex];
                var staticModelModel = ResolveHandler<IMesh, RenderMesh>(staticModel.Model);
                if (staticModelModel != null)
                {
                    if (staticModelModel._vertexBuffer == null)
                        staticModelModel.CreateResources(GraphicsDevice, ResourceFactory);

                    //var rotation =
                    //    Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, _ticks / 1000f)
                    //    * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, _ticks / 3000f);
                    //_cl.UpdateBuffer(_worldBuffer, 0, ref rotation);
                    _cl.UpdateBuffer(_worldBuffer, 0, ref worldTransform.WorldMatrix);

                    _cl.SetVertexBuffer(0, staticModelModel._vertexBuffer);
                    _cl.SetIndexBuffer(staticModelModel._indexBuffer, IndexFormat.UInt16);
                    _cl.SetGraphicsResourceSet(0, _projViewSet);
                    _cl.SetGraphicsResourceSet(1, _worldTextureSet);
                    for (var index = 0; index < staticModelModel.Primitives.Count && index < staticModel.Materials.Count; index++)
                    {
                        var material = ResolveHandler<IMaterial, RenderMaterial>(staticModel.Materials[index]);
                        if (material != null)
                        {
                            var indexRange = staticModelModel.Primitives[index];
                            _cl.UpdateBuffer(_materialBuffer, 0, ref material.DiffuseColor);
                            _cl.DrawIndexed(indexRange.Length, 1, indexRange.Start, 0, 0);
                        }
                    }
                }
            }

            _cl.End();
            GraphicsDevice.SubmitCommands(_cl);
            GraphicsDevice.SwapBuffers(MainSwapchain);
            GraphicsDevice.WaitForIdle();
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
            _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _materialBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            _surfaceTexture = _stoneTexData.CreateDeviceTexture(GraphicsDevice, ResourceFactory);
            _surfaceTextureView = factory.CreateTextureView(_surfaceTexture);

            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float2),
                        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3))
                },
                factory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(new VertexShader().TransformText()), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(new FragmentShader().TransformText()), "main"))
                );

            var projViewLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex)));

            var worldTextureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex, ResourceLayoutElementOptions.None),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly,
                        ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler,
                        ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("BaseColor", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)
                ));

            _pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
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
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] {projViewLayout, worldTextureLayout},
                MainSwapchain.Framebuffer.OutputDescription));

            _projViewSet = factory.CreateResourceSet(new ResourceSetDescription(
                projViewLayout,
                _projectionBuffer,
                _viewBuffer));

            _worldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                worldTextureLayout,
                _worldBuffer,
                _surfaceTextureView,
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
    }
}