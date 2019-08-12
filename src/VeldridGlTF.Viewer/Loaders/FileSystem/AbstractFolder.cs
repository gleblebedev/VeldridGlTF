namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class AbstractFolder
    {
        public string Path { get; }
        public AbstractFolder(string mountPoint)
        {
            Path = mountPoint;
        }
        public string GetChildLogicalPath(string name)
        {
            if (string.IsNullOrWhiteSpace(Path))
                return name;
            return Path + "/" + name;
        }

    }
}