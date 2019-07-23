using System;
using System.Collections.Generic;

namespace Leger
{
    public interface IIndexManager
    {
        void IndexVertex(IVertex vertex);

        IVertexIndex GetIndex(string indexName);
        List<string> GetIndexNames();

        void RebuildIndexes(IEnumerable<IVertex> vertices, Func<IVertex, IEnumerable<IndexedString>> customLogic, bool overrideExistingLogic);
    }
}
