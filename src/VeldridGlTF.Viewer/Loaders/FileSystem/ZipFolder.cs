using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class ZipFolder : IFolder
    {
        private readonly Dictionary<string, ManualResourceHandler<IFile>> _files =
            new Dictionary<string, ManualResourceHandler<IFile>>();

        private readonly Dictionary<string, IFolder> _folders = new Dictionary<string, IFolder>();
        private readonly object _gate = new object();


        public ZipFolder(string mountPoint)
        {
            Path = mountPoint;
        }

        public string Path { get; }

        public IFolder GetFolder(string folderName)
        {
            lock (_gate)
            {
                IFolder folder;
                if (_folders.TryGetValue(folderName, out folder))
                    return folder;
                folder = new ZipFolder(folderName);
                _folders.Add(folderName, folder);
                return folder;
            }
        }

        public void Mount(string folderName, IFolder target)
        {
            lock (_gate)
            {
                _folders[folderName] = target;
            }
        }

        public IResourceHandler<IFile> GetFile(string fileName)
        {
            lock (_gate)
            {
                if (_files.TryGetValue(fileName, out var file))
                    return file;
                return new ManualResourceHandler<IFile>(new ResourceId(GetChildLogicalPath(fileName)),
                    new FileNotFoundException("File " + fileName + " is not found in zip file", fileName));
            }
        }

        private string GetChildLogicalPath(string name)
        {
            if (string.IsNullOrWhiteSpace(Path))
                return name;
            return Path + "/" + name;
        }

        public void SetFile(string fileName, ZipArchiveEntry entry)
        {
            _files.Add(fileName,
                new ManualResourceHandler<IFile>(new ResourceId(GetChildLogicalPath(fileName)), new ZipFile(entry)));
        }
    }
}