namespace Veldrid.SPIRV
{
    public class SpirvCompilationResultEx: SpirvCompilationResult
    {
        private readonly ResourceLayoutDescription[] _layouts;

        public SpirvCompilationResultEx(byte[] spirvBytes, ResourceLayoutDescription[] layouts) : base(spirvBytes)
        {
            _layouts = layouts;
        }

        public ResourceLayoutDescription[] Layouts
        {
            get { return _layouts; }
        }
    }
}