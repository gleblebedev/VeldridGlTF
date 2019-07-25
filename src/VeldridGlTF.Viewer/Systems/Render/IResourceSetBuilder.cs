namespace VeldridGlTF.Viewer.Systems.Render
{
    public interface IResourceSetBuilder
    {
        bool TryResolve(string name, out ResourceSetSlot slot);
        ResourceSetSlot[] Resolve(params string[] resources);
    }
}