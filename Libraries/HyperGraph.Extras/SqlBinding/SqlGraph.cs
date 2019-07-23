using System;
using System.Data.Common;
using System.Collections.Generic;

using Leger;

namespace Leger.Extra.SqlBinding
{
    public class SqlGraph<ConcreteCommand> : DataStoreBackedGraph where ConcreteCommand : DbCommand, new()
    {
        SqlVertexStore<ConcreteCommand> vertexStore;

        public SqlGraph(DbProvider<ConcreteCommand> p,
            Func<IVertex, IEnumerable<IndexedString>> indexConstructionLogic = null)
           : base(p.VertexStore, p.EdgeStore, indexConstructionLogic)
        {
            vertexStore = p.VertexStore;
        }

        public override List<IVertex> SearchNode(string indexName, string searchString)
        {
            return vertexStore.SearchNode(searchString);
        }
    }
}