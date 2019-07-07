using System.IO;
using VeldridGlTF.Viewer.Data;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class FilesystemFile : IFile
    {
        private readonly string _path;

        public FilesystemFile(string path)
        {
            _path = path;
        }

        public Stream Open()
        {
            return File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}