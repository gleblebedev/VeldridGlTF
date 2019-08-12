using VeldridGlTF.Viewer.Data;
using VeldridGlTF.Viewer.Resources;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public interface IFolder
    {
        string Path { get; }
        IFolder GetFolder(string folderName);
        IResourceHandler<IFile> GetFile(string fileName);
        void Mount(string folderName, IFolder target);
        string GetChildLogicalPath(string name);
    }
}