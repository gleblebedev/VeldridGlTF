namespace Veldrid.SPIRV
{
    public interface ILayoutNameResolver
    {
        string Resolve(uint set, uint binding, ResourceKind kind, out ResourceLayoutElementOptions options);
        ResourceLayoutElementOptions Resolve(string name, uint set, uint binding, ResourceKind kind);
    }
}