using System.IO;

namespace VeldridGlTF.Viewer.Data
{
    public interface ITexture
    {
    }

    public interface IFile
    {
        Stream Open();
    }
}