using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Leopotam.Ecs;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.SPIRV;
using VeldridGlTF.Viewer.Components;

namespace VeldridGlTF.Viewer.Systems.Render
{
    [EcsInject]
    public class VeldridRenderSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem
    {
        private readonly StepContext _stepContext;

        #region Shaders
        private const string VertexCode = @"
#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 Projection;
};

layout(set = 0, binding = 1) uniform ViewBuffer
{
    mat4 View;
};

layout(set = 1, binding = 0) uniform WorldBuffer
{
    mat4 World;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 0) out vec2 fsin_texCoords;

void main()
{
    vec4 worldPosition = World * vec4(Position, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
    fsin_texCoords = TexCoords;
}";

        private const string FragmentCode = @"
#version 450

layout(location = 0) in vec2 fsin_texCoords;
layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;

void main()
{
    fsout_color =  texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
}";
        #endregion

        EcsWorld _world = null;
        EcsFilter<WorldTransform, StaticModel> _staticModels = null;

        protected Camera _camera;
        private CommandList _cl;
        private Pipeline _pipeline;
        private DeviceBuffer _projectionBuffer;
        private ResourceSet _projViewSet;
        private ImageSharpTexture _stoneTexData;
        private Texture _surfaceTexture;
        private TextureView _surfaceTextureView;
        private float _ticks;

        private DeviceBuffer _viewBuffer;
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

         

            _surfaceTexture = _stoneTexData.CreateDeviceTexture(GraphicsDevice, ResourceFactory);
            _surfaceTextureView = factory.CreateTextureView(_surfaceTexture);

            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float2))
                },
                factory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(VertexCode), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main")));

            var projViewLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex)));

            var worldTextureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly,
                        ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler,
                        ShaderStages.Fragment)));

            _pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerStateDescription.Default,
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
                GraphicsDevice.Aniso4xSampler));

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

        public void Run()
        {

            var deltaSeconds = _stepContext.DeltaSeconds;
            _ticks += deltaSeconds * 1000f;

            _camera.Yaw += deltaSeconds;
            _camera.Position = _camera.Forward * -100;

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

            foreach (var modelIndex in _staticModels)
            {
                var worldTransform = _staticModels.Components1[modelIndex];
                var staticModel = _staticModels.Components2[modelIndex];
                if (staticModel.Model.Status == TaskStatus.RanToCompletion)
                {
                    var staticModelModel = staticModel.Model.GetAsync().Result as RenderMesh;
                    if (staticModelModel != null)
                    {
                        if (staticModelModel._vertexBuffer == null)
                        {
                            staticModelModel.CreateResources(GraphicsDevice, ResourceFactory);
                        }

                        //var rotation =
                        //    Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, _ticks / 1000f)
                        //    * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, _ticks / 3000f);
                        //_cl.UpdateBuffer(_worldBuffer, 0, ref rotation);
                        _cl.UpdateBuffer(_worldBuffer, 0, ref worldTransform.WorldMatrix);

                        _cl.SetPipeline(_pipeline);
                        _cl.SetVertexBuffer(0, staticModelModel._vertexBuffer);
                        _cl.SetIndexBuffer(staticModelModel._indexBuffer, IndexFormat.UInt16);
                        _cl.SetGraphicsResourceSet(0, _projViewSet);
                        _cl.SetGraphicsResourceSet(1, _worldTextureSet);
                        _cl.DrawIndexed(staticModelModel.IndexCount, 1, 0, 0, 0);
                    }
                }
            }

            _cl.End();
            GraphicsDevice.SubmitCommands(_cl);
            GraphicsDevice.SwapBuffers(MainSwapchain);
            GraphicsDevice.WaitForIdle();
        }

        
    }
}