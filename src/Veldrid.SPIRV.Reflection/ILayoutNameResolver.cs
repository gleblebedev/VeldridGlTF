namespace Veldrid.SPIRV
{
    public interface ILayoutNameResolver
    {
        string Resolve(uint set, uint binding, ResourceKind kind);
    }
}