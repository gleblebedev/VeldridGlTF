using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Data.Geometry;
using VeldridGlTF.Viewer.Loaders.FileSystem;
using VeldridGlTF.Viewer.Loaders.GlTF;
using VeldridGlTF.Viewer.Resources;
using VeldridGlTF.Viewer.SceneGraph;
using VeldridGlTF.Viewer.Systems.Render;
using VeldridGlTF.Viewer.Systems.Render.Resources;

namespace VeldridGlTF.Viewer
{
    public class SceneRenderer : IDisposable
    {
        private readonly object _gate = new object();
        private readonly ResourceManager _resourceManager;
        private readonly Scene _scene = new Scene();
        private readonly StepContext _stepContext;

        private readonly VeldridRenderSystem _veldridRenderSystem;

        public SceneRenderer(IApplicationWindow window, ViewerOptions options)
        {
            Window = window;

            _stepContext = new StepContext();
            _veldridRenderSystem = new VeldridRenderSystem(_stepContext, window);
            _scene.Systems
                .Add(new LocalToWorldSystem(_scene))
                .Add(_veldridRenderSystem);
            _scene.Render = _veldridRenderSystem;
            _scene.Systems.Initialize();
            FileCollection fileCollection;
            if (options.RootFolder != null)
                fileCollection = new FileCollection(options.RootFolder);
            else
                fileCollection = new FileCollection(options.DataFolder);
            _resourceManager = new ResourceManager()
                    .With(fileCollection)
                    .With(new ContainerLoader(), ".glb", ".gltf")
                    .With(new TextureLoader(_veldridRenderSystem))
                    .With(new MaterialLoader(_veldridRenderSystem))
                    .With(new MeshLoader(_veldridRenderSystem))
                ;
            Task.Run(() => LoadGlTFSample(options.FileName, options.Scale));

            Window.Rendering += PreDraw;
            Window.Rendering += Draw;
        }

        public IApplicationWindow Window { get; }

        public void Dispose()
        {
            _scene.Dispose();
        }

        private async Task<Node> LoadGlTFSample(string name, float scale)
        {
            var resourceId = new ResourceId(name, null);

            var prefab = await _resourceManager.Resolve<EntityPrefab>(resourceId).GetAsync();
            //{
            //    var node = prefab.Children[0];
            //    var mesh = await _resourceManager.Resolve<IGeometry>(node.Mesh.Id).GetAsync();
            //    //PrintGeometry(mesh, prefab.LocalMatrix * node.LocalMatrix);
            //}
            lock (_gate)
            {
                var node = prefab.Spawn(_scene);
                node.Transform.Scale = Vector3.One * scale;
                return node;
            }
        }

        private static void PrintGeometry(IGeometry mesh, Matrix4x4? transform = null)
        {
            var geometryPrimitive = mesh.Primitives[0];
            var t = transform ?? Matrix4x4.Identity;
            foreach (var stream in geometryPrimitive.Streams)
            {
                IEnumerable<string> values = null;

                switch (stream.Key)
                {
                    case "POSITION":
                        values = stream.AsVector3().Select(_=>Vector3.Transform(_,t)).Select(_ => $"new Vector3({_.X}f, {_.Y}f, {_.Z}f)");
                        break;
                    case "NORMAL":
                        values = stream.AsVector3().Select(_ => Vector3.TransformNormal(_, t)).Select(_ => $"new Vector3({_.X}f, {_.Y}f, {_.Z}f)");
                        break;
                    case "TANGENT":
                        values = stream.AsVector4().Select(_ => TransformTangent(_, t)).Select(_ => $"new Vector4({_.X}f, {_.Y}f, {_.Z}f, {_.W}f)");
                        break;
                    default:
                        switch (stream.Format)
                        {
                            case GeometryStreamFormat.Float1:
                                values = stream.AsScalar().Select(_ => _.ToString()+"f");
                                break;
                            case GeometryStreamFormat.Float2:
                                values = stream.AsVector2().Select(_ => $"new Vector2({_.X}f, {_.Y}f)");
                                break;
                            case GeometryStreamFormat.Float3:
                                values = stream.AsVector3().Select(_ => $"new Vector3({_.X}f, {_.Y}f, {_.Z}f)");
                                break;
                            case GeometryStreamFormat.Float4:
                                values = stream.AsVector4().Select(_ => $"new Vector4({_.X}f, {_.Y}f, {_.Z}f, {_.W}f)");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                }

                Debug.WriteLine("AbstractGeometryStream.Create(\"" + stream.Key + "\", new []{" + string.Join(", ", values) + "}),");
            }
            Debug.WriteLine(string.Join(", ", geometryPrimitive.Indices));
        }

        private static Vector4 TransformTangent(Vector4 _, Matrix4x4 t)
        {
            var tangent = Vector3.TransformNormal(new Vector3(_.X,_.Y, _.Z), t);
            return new Vector4(tangent.X, tangent.Y, tangent.Z, _.W);
        }

        private void PreDraw(float deltaSeconds)
        {
        }

        protected void Draw(float deltaSeconds)
        {
            _stepContext.DeltaSeconds = deltaSeconds;
            lock (_gate)
            {
                _scene.Systems.Run();
            }
        }
    }
}