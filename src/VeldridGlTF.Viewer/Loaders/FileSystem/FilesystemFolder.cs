﻿using System.Collections.Generic;
using System.IO;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class FilesystemFolder : AbstractFolder, IFolder
    {
        private readonly Dictionary<string, ManualResourceHandler<IFile>> _files =
            new Dictionary<string, ManualResourceHandler<IFile>>();

        private readonly Dictionary<string, IFolder> _folders = new Dictionary<string, IFolder>();
        private readonly object _gate = new object();
        private readonly string _physicalPath;

        public FilesystemFolder(string physicalPath, string logicalPath) : base(logicalPath)
        {
            _physicalPath = physicalPath;
        }

        public IResourceHandler<IFile> GetFile(string fileName)
        {
            lock (_gate)
            {
                if (_files.TryGetValue(fileName, out var file))
                    return file;
                var fullFilePath = System.IO.Path.Combine(_physicalPath, fileName);
                if (!File.Exists(fullFilePath))
                    file = new ManualResourceHandler<IFile>(new ResourceId(GetChildLogicalPath(fileName)),
                        new FileNotFoundException("File " + fileName + " not found", fullFilePath));
                else
                    file = new ManualResourceHandler<IFile>(new ResourceId(GetChildLogicalPath(fileName)),
                        new FilesystemFile(fullFilePath));
                _files.Add(fileName, file);
                return file;
            }
        }

        public void Mount(string folderName, IFolder target)
        {
            lock (_gate)
            {
                _folders[folderName] = target;
            }
        }

        public IFolder GetFolder(string folderName)
        {
            lock (_gate)
            {
                IFolder folder;
                if (_folders.TryGetValue(folderName, out folder))
                    return folder;
                folder = new FilesystemFolder(System.IO.Path.Combine(_physicalPath, folderName),
                    GetChildLogicalPath(folderName));
                _folders.Add(folderName, folder);
                return folder;
            }
        }

        public override string ToString()
        {
            return _physicalPath;
        }
    }
}