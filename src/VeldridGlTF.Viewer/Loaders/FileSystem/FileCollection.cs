using System;
using System.Collections.Generic;
using System.IO;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class FileCollection : IResourceCollection<IFile>
    {
        private readonly IFolder _rootFolder;

        public FileCollection(string rootFolder)
        {
            _rootFolder = new FilesystemFolder(Path.GetFullPath(rootFolder), "");
        }

        public FileCollection(IFolder rootFolder)
        {
            if (!string.IsNullOrEmpty(rootFolder.Path))
                throw new ArgumentException("Root folder should be mounted at root path");
            _rootFolder = rootFolder;
        }

        public IResourceHandler<IFile> Resolve(ResourceId id)
        {
            if (id.Id != null) throw new ArgumentException("File resource identifier can't have Id part.");

            var folder = _rootFolder;
            string fileName = null;
            foreach (var segment in GetSegments(id.Path))
            {
                if (fileName != null) folder = folder.GetFolder(fileName);

                fileName = segment;
            }

            return folder.GetFile(fileName);
        }

        public void Mount(IFolder target)
        {
            var folder = _rootFolder;
            string fileName = null;
            foreach (var segment in GetSegments(target.Path))
            {
                if (fileName != null) folder = folder.GetFolder(fileName);

                fileName = segment;
            }

            folder.Mount(fileName, target);
        }

        internal static IEnumerable<string> GetSegments(string targetPath)
        {
            var startIndex = 0;
            var endIndex = 0;
            while (endIndex < targetPath.Length)
            {
                if (targetPath[endIndex] == '/')
                {
                    yield return targetPath.Substring(startIndex, endIndex - startIndex);
                    startIndex = endIndex + 1;
                }

                ++endIndex;
            }

            if (startIndex < endIndex) yield return targetPath.Substring(startIndex, endIndex - startIndex);
        }
    }
}