using System.IO;

namespace VeldridGlTF.Viewer.Loaders.FileSystem
{
    public static class ExtensionMethods
    {
        public static void MountZip(this IFolder folder, string mountPoint, string zipFile)
        {
            var childLogicalPath = folder.GetChildLogicalPath(mountPoint);
            folder.Mount(mountPoint, new ZipMount(zipFile,  childLogicalPath));
        }
        public static void MountZip(this IFolder folder, string zipFile)
        {
            folder.MountZip(Path.GetFileNameWithoutExtension(zipFile), zipFile);
        }
    }
}