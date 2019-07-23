using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Leger.Extra.SqlBinding
{
    internal class SqlGraphObjectStore<ConcreteDbCommand> : IDisposable where ConcreteDbCommand : DbCommand, new()
    {
        protected DbConnection connection;
        protected Dictionary<string, GraphObjectTypeInfo> types = new Dictionary<string, GraphObjectTypeInfo>();
        protected string table;
        protected DbProvider<ConcreteDbCommand> provider;

        public SqlGraphObjectStore(DbConnection connection, string tableName)
        {
            this.connection = connection;
            table = tableName;

            using (DbCommand req = (ConcreteDbCommand)Activator.CreateInstance(typeof(ConcreteDbCommand), new object[] { $"SELECT * FROM Type", connection }))
            {
                using (DbDataReader reader = req.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader["id"].ToString();
                        var t = new GraphObjectTypeInfo(Guid.Parse(id),
                            reader["name"].ToString(), (bool)reader["is_vertex"] ? GraphObjectType.Vertex : GraphObjectType.Edge);
                        types.Add(id, t);
                    }
                    reader.Close();
                }
            }
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
