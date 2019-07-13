using System;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Default
{
    public class DefaultShaderFactory: IShaderFactory
    {
        public AbstractShaderKey GetShaderKey(PipelineKey pipelineKey)
        {
            throw new NotImplementedException();
        }

        public IShaderGenerator ResolveGenerator(ShaderKey key)
        {
            return new DefaultShaderGenerator(key);
        }
    }
}