namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class AbstractFolder
    {
        public AbstractFolder(string mountPoint)
        {
            Path = mountPoint;
        }

        public string Path { get; }

        public string GetChildLogicalPath(string name)
        {
            if (string.IsNullOrWhiteSpace(Path))
                return name;
            return Path + "/" + name;
        }
    }
}