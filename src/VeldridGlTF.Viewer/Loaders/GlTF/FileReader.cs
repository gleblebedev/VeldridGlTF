using System;
using System.IO;
using System.Net;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.GlTF
{
    public class FileReader
    {
        private readonly ResourceContext _context;

        public FileReader(ResourceContext context)
        {
            _context = context;
        }

        public ArraySegment<byte> ReadAsset(string assetname)
        {
            var fileName = Path.Combine(Path.GetDirectoryName(_context.Id.Path), WebUtility.UrlDecode(assetname))
                .Replace(Path.DirectorySeparatorChar, '/');
            var file = _context.Resolve<IFile>(new ResourceId(fileName, null)).GetAsync().Result;
            using (var f = file.Open())
            {
                var buf = new MemoryStream();
                f.CopyTo(buf);
                return new ArraySegment<byte>(buf.ToArray());
            }
        }
    }
}