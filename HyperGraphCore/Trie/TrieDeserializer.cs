using Leger.IO;

namespace Leger.Extra
{
    public static class TrieDeserializer
    {
        public static Trie GetTrieInstance(string resourcePath, IFileProvider fileProvider)
        {
            Graph g = GraphXmlDeserializer.GetGraphInstance(resourcePath, fileProvider, ExternalRessourcesLoadingPolicy.None);
            return new Trie(g);
        }
    }
}
