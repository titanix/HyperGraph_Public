using System;
using System.Collections.Generic;

namespace Leger
{
    internal class DictionaryIndex : IVertexIndex
    {
        Dictionary<string, List<IVertex>> data = new Dictionary<string, List<IVertex>>();
        Func<IVertex, IEnumerable<IndexedString>> indexingLogic;

        public string Name { get; private set; }

        internal DictionaryIndex(string name, Func<IVertex, IEnumerable<IndexedString>> indexingLogic)
        {
            this.Name = name;
            this.indexingLogic = indexingLogic;
        }

        public IEnumerable<string> IndexedStrings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void IndexVertex(IVertex vertex)
        {
            foreach (IndexedString s in indexingLogic(vertex))
            {
                if (data.ContainsKey(s.Value))
                {
                    data[s.Value].Add(vertex);
                }
                else
                {
                    data.Add(s.Value, new List<IVertex>() { vertex });
                }
            }
        }

        public void Rebuild(Action<IGraph> logic)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<IVertex>> ToDictionary()
        {
            return data;
        }
    }
}
