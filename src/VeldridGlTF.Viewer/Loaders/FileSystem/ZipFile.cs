using System.IO;
using System.IO.Compression;
using VeldridGlTF.Viewer.Data;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class ZipFile : IFile
    {
        private readonly ZipArchiveEntry _entry;

        public ZipFile(ZipArchiveEntry entry)
        {
            _entry = entry;
        }

        public Stream Open()
        {
            return _entry.Open();
        }
    }
}