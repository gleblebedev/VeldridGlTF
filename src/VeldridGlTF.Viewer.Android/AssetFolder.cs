using System.IO;
using Android.Content.Res;
using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Loaders.FileSystem;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Android
{
    public class AssetFolder : IFolder
    {
        public AssetFolder(AssetManager assetManager, string path)
        {
            _assetManager = assetManager;
            _path = path;
        }

        private readonly AssetManager _assetManager;
        private string _path;

        public string Path => _path;

        public IFolder GetFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(_path))
                return new AssetFolder(_assetManager, folderName);
            return new AssetFolder(_assetManager, _path + "/"+folderName);
        }

        public IResourceHandler<IFile> GetFile(string fileName)
        {
            string id = fileName;
            if (!string.IsNullOrWhiteSpace(_path))
                id = _path + "/" + fileName;
            return new ManualResourceHandler<IFile>(new ResourceId(id), new AssetFile(_assetManager, id));
        }

        public void Mount(string folderName, IFolder target)
        {
            throw new System.NotImplementedException();
        }
    }

    public class AssetFile:IFile
    {
        private readonly AssetManager _assetManager;
        private readonly string _id;

        public AssetFile(AssetManager assetManager, string id)
        {
            _assetManager = assetManager;
            _id = id;
        }

        public Stream Open()
        {
            return _assetManager.Open(_id);
        }
    }
}