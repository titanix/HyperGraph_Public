using System;
using System.Data.Common;

namespace Leger.Extra.SqlBinding
{
    public class DbProvider<ConcreteDbCommand> : IGraphStore where ConcreteDbCommand : DbCommand, new()
    {
        SqlVertexStore<ConcreteDbCommand> vertexStore;
        SqlEdgeStore<ConcreteDbCommand> edgeStore;
        bool useLookupTable = false;
        DbConnection connection;

        public DbProvider(
            DbConnection connection,
            IVertexStoreRawSqlProvider vertexSqlProv,
            IEdgeStoreRawSqlProvider edgeSqlProv,
            bool useLookupTable = false)
        {
            vertexStore = new SqlVertexStore<ConcreteDbCommand>(connection, this, vertexSqlProv);
            edgeStore = new SqlEdgeStore<ConcreteDbCommand>(connection, this, edgeSqlProv);
            this.connection = connection;
            this.useLookupTable = useLookupTable;
        }

        internal SqlVertexStore<ConcreteDbCommand> VertexStore => vertexStore;
        internal SqlEdgeStore<ConcreteDbCommand> EdgeStore => edgeStore;

        public IGraphObject GetObjectById(Guid id)
        {
            IGraphObject result = null;

            if (useLookupTable)
            {
                object queryResult = connection.ExecuteScalar<ConcreteDbCommand>($"SELECT lookup_table FROM IdType WHERE id = '{id}'");

                if (queryResult != null)
                {
                    string table = queryResult.ToString();
                    switch (table)
                    {
                        case "Vertex":
                            return vertexStore.GetElementById(id);
                        default:
                            return edgeStore.GetElementById(id, table);
                    }
                }
            }
            else
            {
                try
                {
                    result = vertexStore.GetElementById(id);
                }
                catch
                {
                    result = edgeStore.GetElementById(id);
                }

                if (result == null)
                {
                    result = edgeStore.GetElementById(id);
                }
            }
            return result;
        }
    }
}