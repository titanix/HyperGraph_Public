using System;
using System.Collections.Generic;

namespace Leger
{
    class IndexManager : IIndexManager
    {
        protected bool indexDirty = true;

        //Dictionary<string, Dictionary<string, List<IVertex>>> globalIndexes = new Dictionary<string, Dictionary<string, List<IVertex>>>();
        protected Dictionary<string, IVertexIndex> globalIndexes = new Dictionary<string, IVertexIndex>();

        protected Func<IVertex, IEnumerable<IndexedString>> indexBuildingLogic;

        public IndexManager() : this(null) { }

        public IndexManager(Func<IVertex, IEnumerable<IndexedString>> indexBuildingLogic)
        {
            if (indexBuildingLogic == null)
            {
                this.indexBuildingLogic = v =>
                {
                    return v.IndexableStrings;
                };
            }
            else
            {
                this.indexBuildingLogic = indexBuildingLogic;
            }
        }

        public IVertexIndex GetIndex(string indexName)
        {
            //RebuildIndexes();

            if (globalIndexes.ContainsKey(indexName))
            {
                return globalIndexes[indexName];
            }
            else
            {
                return new DictionaryIndex(indexName, indexBuildingLogic);
            }
        }

        public List<string> GetIndexNames()
        {
            return new List<string>(globalIndexes.Keys);
        }

        public void RebuildIndexes(
            IEnumerable<IVertex> vertices,
            Func<IVertex, IEnumerable<IndexedString>> customLogic,
            bool overrideExistingLogic = false)
        {
            if (overrideExistingLogic)
            {
                indexBuildingLogic = customLogic;
            }
            if (indexDirty)
            {
                globalIndexes.Clear();
                foreach (IVertex node in vertices)
                {
                    IndexVertex(node);
                }
                indexDirty = false;
            }
        }

        public void IndexVertex(IVertex node)
        {
            IEnumerable<IndexedString> indexedStrings = indexBuildingLogic(node);

            foreach (IndexedString iString in indexedStrings)
            {
                // on crée l'index s'il n'existe pas
                if (!globalIndexes.ContainsKey(iString.IndexName))
                {
                    globalIndexes.Add(iString.IndexName, new DictionaryIndex(iString.IndexName, null));
                }
                // on récupère une référence à l'index à remplir
                Dictionary<string, List<IVertex>> index = globalIndexes[iString.IndexName].ToDictionary();
                // on ajoute le noeud à la liste des noeuds indexé pour une chaîne donnée
                if (index.ContainsKey(iString.Value))
                {
                    index[iString.Value].Add(node);
                }
                else
                {
                    List<IVertex> nodeList = new List<IVertex>();
                    nodeList.Add(node);
                    index.Add(iString.Value, nodeList);
                }
            }
        }
    }
}
