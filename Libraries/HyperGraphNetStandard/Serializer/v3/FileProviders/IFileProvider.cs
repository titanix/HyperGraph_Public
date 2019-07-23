namespace Leger.IO
{
    using System.IO;

    /// <summary>
    /// Fournit un flux textuel à partir d'un emplacement de fichier.
    /// </summary>
    public interface IFileProvider
    {
        StreamReader GetFileReader(string path);
    }
}