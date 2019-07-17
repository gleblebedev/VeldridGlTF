using System.IO;

namespace VeldridGlTF.Viewer.Data
{
    public interface IFile
    {
        Stream Open();
    }
}