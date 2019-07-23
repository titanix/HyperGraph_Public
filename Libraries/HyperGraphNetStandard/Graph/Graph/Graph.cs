using System;
using System.Collections.Generic;

namespace Leger
{
    public class Graph : DataStoreBackedGraph
    {
        public Graph(Func<IVertex, IEnumerable<IndexedString>> indexConstructionLogic = null) 
            : base(new DictionaryRepository<IVertex>(), new DictionaryRepository<IEdge>(), indexConstructionLogic) { }
    }
}
