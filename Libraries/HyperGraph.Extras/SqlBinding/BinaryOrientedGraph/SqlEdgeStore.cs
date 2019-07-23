using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Leger.Extra.SqlBinding
{
    internal class SqlEdgeStore<ConcreteCommand> : SqlGraphObjectStore<ConcreteCommand>, IGraphObjectRepository<IEdge> where ConcreteCommand : DbCommand, new()
    {
        private Dictionary<Guid, IEdge> cache = new Dictionary<Guid, IEdge>();
        private IEdgeStoreRawSqlProvider rawSqlProvider;

        public SqlEdgeStore(DbConnection dbConnection, DbProvider<ConcreteCommand> provider, IEdgeStoreRawSqlProvider edgeStoreRawSqlProvider)
            : base(dbConnection, "Edge")
        {
            this.provider = provider;
            rawSqlProvider = edgeStoreRawSqlProvider;
        }

        public int Count => throw new NotImplementedException();

        public IEnumerable<IEdge> Values => throw new NotImplementedException();

        public void DeleteIfExists(IEdge o)
        {
            throw new NotImplementedException();
        }

        public IEdge GetElementById(Guid id)
        {
            return GetElementById(id, table);
        }

        public IEdge GetElementById(Guid id, string table)
        {
            if (!cache.ContainsKey(id))
            {
                string sql = String.Format(rawSqlProvider.SelectById, table, id);
                using (DbCommand command = (ConcreteCommand)Activator.CreateInstance(typeof(ConcreteCommand), new object[] { sql, connection }))
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            GraphObjectTypeInfo ti = types[reader["type_id"].ToString()];
                            Guid source = Guid.Parse(reader["source"].ToString());
                            Guid target = Guid.Parse(reader["target"].ToString());

                            IEdge edge = new HyperEdge(id, ti, provider, new List<Guid>() { source, target });
                            cache.Add(id, edge);
                        }
                        else
                        {
                            throw new Exception("Missing edge data.");
                        }
                    }
                }
            }
            return cache[id];
        }

        public void Cache(IEdge edge)
        {
            if (!cache.ContainsKey(edge.ObjectId))
            {
                cache.Add(edge.ObjectId, edge);
            }
        }

        public IEnumerator<IEdge> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void StoreIfNotExists(IEdge o)
        {
            throw new NotImplementedException();
        }
    }
}