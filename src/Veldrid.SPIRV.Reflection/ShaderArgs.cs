namespace Veldrid.SPIRV
{
    public class ShaderArgs
    {
        public string Source { get; set; }
        public string FileName { get; set; } = "shader.glsl";
        public ShaderStages Stage { get; set; }
    }
}