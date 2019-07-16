using System;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class Skybox: IDisposable
    {
        const float halfSize = 1.0f;
        private static Vector3[] _vertices = new Vector3[]
        {
        
        new Vector3(-halfSize, +halfSize, -halfSize),
        new Vector3(+halfSize, +halfSize, -halfSize),
        new Vector3(+halfSize, +halfSize, +halfSize),
        new Vector3(-halfSize, +halfSize, +halfSize),
                                                    
        new Vector3(-halfSize, -halfSize, +halfSize),
        new Vector3(+halfSize, -halfSize, +halfSize),
        new Vector3(+halfSize, -halfSize, -halfSize),
        new Vector3(-halfSize, -halfSize, -halfSize),
                                                    
        new Vector3(-halfSize, +halfSize, -halfSize),
        new Vector3(-halfSize, +halfSize, +halfSize),
        new Vector3(-halfSize, -halfSize, +halfSize),
        new Vector3(-halfSize, -halfSize, -halfSize),
                                                    
        new Vector3(+halfSize, +halfSize, +halfSize),
        new Vector3(+halfSize, +halfSize, -halfSize),
        new Vector3(+halfSize, -halfSize, -halfSize),
        new Vector3(+halfSize, -halfSize, +halfSize),
                                                    
        new Vector3(+halfSize, +halfSize, -halfSize),
        new Vector3(-halfSize, +halfSize, -halfSize),
        new Vector3(-halfSize, -halfSize, -halfSize),
        new Vector3(+halfSize, -halfSize, -halfSize),
                                                    
        new Vector3(-halfSize, +halfSize, +halfSize),
        new Vector3(+halfSize, +halfSize, +halfSize),
        new Vector3(+halfSize, -halfSize, +halfSize),
        new Vector3(-halfSize, -halfSize, +halfSize),
        };
        private static ushort[] indices = new ushort[]
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };

        private Texture _cubemap;
        private TextureView _cubemapView;

        public Skybox(VeldridRenderSystem render, ImageSharpCubemapTexture cubemapTexture)
        {
            var context = render.RenderContext.GetAsync().Result;
            var factory = context.Factory;
            _cubemap = cubemapTexture.CreateDeviceTexture(context.Device, factory);
            _cubemapView = factory.CreateTextureView(_cubemap);

            var vertexBuffer =
                factory.CreateBuffer(new BufferDescription(
                    (uint)_vertices.Length*12, BufferUsage.VertexBuffer));
            context.Device.UpdateBuffer(vertexBuffer, 0, _vertices);

            var indexBuffer =
                factory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint)indices.Length,
                    BufferUsage.IndexBuffer));
            context.Device.UpdateBuffer(indexBuffer, 0, indices);


        }
        public void Dispose()
        {
            
        }

        public void Render(CommandList cl)
        {
            cl.ClearColorTarget(0, new RgbaFloat(48.0f / 255.0f, 10.0f / 255.0f, 36.0f / 255.0f, 1));
        }
    }
}