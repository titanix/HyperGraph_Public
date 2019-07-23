using System.IO;

namespace Leger.IO
{
    public class FileLoader : IFileProvider
    {
        public StreamReader GetFileReader(string path)
        {
            return new StreamReader(path);
        }
    }
}