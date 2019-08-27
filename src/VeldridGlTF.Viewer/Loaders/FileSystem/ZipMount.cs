using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public class ZipMount : ZipFolder, IDisposable
    {
        private readonly ZipArchive _archive;
        private readonly string _archiveFileName;
        private readonly FileStream _stream;
        private Dictionary<string, IFolder> _folders = new Dictionary<string, IFolder>();

        public ZipMount(string archiveFileName, string mountPoint) : base(mountPoint)
        {
            _archiveFileName = archiveFileName;
            _stream = File.Open(_archiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _archive = new ZipArchive(_stream);
            foreach (var entry in _archive.Entries)
                if (!string.IsNullOrEmpty(entry.Name))
                    RegisterEntry(entry);
        }

        public void Dispose()
        {
            _archive.Dispose();
            _stream.Dispose();
        }

        private void RegisterEntry(ZipArchiveEntry entry)
        {
            IFolder folder = this;
            string fileName = null;
            foreach (var segment in FileCollection.GetSegments(entry.FullName))
            {
                if (fileName != null) folder = folder.GetFolder(fileName);

                fileName = segment;
            }

            ((ZipFolder) folder).SetFile(fileName, entry);
        }

        public override string ToString()
        {
            return _archiveFileName;
        }
    }
}