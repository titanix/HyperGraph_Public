#if NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471
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
#endif