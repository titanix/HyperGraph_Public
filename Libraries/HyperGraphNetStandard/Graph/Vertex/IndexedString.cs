namespace Leger
{
    public sealed class IndexedString
    {
        public string IndexName { get; private set; }

        public string Value { get; private set; }

        /// <summary>
        /// Si aucun nom d'index n'est donné, on place la chaîne dans l'index global.
        /// </summary>
        /// <param name="value"></param>
        public IndexedString(string value)
        {
            IndexName = "";
            Value = value;
        }

        public IndexedString(string indexName, string value)
        {
            IndexName = indexName;
            Value = value;
        }
    }
}
