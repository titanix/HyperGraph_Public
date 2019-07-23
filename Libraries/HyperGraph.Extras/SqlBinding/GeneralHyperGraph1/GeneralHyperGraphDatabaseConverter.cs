using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Leger.Extra.SqlBinding
{
    public class GeneralHyperGraphDatabaseConverter<ConcreteDbCommand> : IDatabaseCreator where ConcreteDbCommand : DbCommand, new()
    {
        private DbConnection connection;

        private Dictionary<Guid, string> lookupTables = new Dictionary<Guid, string>();

        public GeneralHyperGraphDatabaseConverter(DbConnection connection) => this.connection = connection;

        public void CreateDatabase()
        {
            connection.Open();

            DbCommand createTable = CreateConcreteCommand(
                "CREATE TABLE Type (name TEXT NOT NULL, id TEXT NOT NULL PRIMARY KEY, description TEXT NOT NULL, is_vertex BOOLEAN)", connection);
            createTable.ExecuteNonQuery();

            createTable = CreateConcreteCommand("CREATE TABLE IdType (id TEXT NOT NULL, lookup_table TEXT NOT NULL)", connection);
            createTable.ExecuteNonQuery();

            createTable = CreateConcreteCommand(
                "CREATE TABLE Vertex (id TEXT NOT NULL PRIMARY KEY, type_id TEXT, content TEXT, FOREIGN KEY(type_id) REFERENCES Type(id))", connection);
            createTable.ExecuteNonQuery();

            createTable = CreateConcreteCommand(
                @"CREATE TABLE EdgeB (id TEXT NOT NULL PRIMARY KEY, type_id TEXT, source TEXT, target TEXT,
	                FOREIGN KEY(source) REFERENCES Vertex(id), 
	                FOREIGN KEY(target) REFERENCES Vertex(id),
	                FOREIGN KEY(type_id) REFERENCES Type(id))", connection);
            createTable.ExecuteNonQuery();

            createTable = CreateConcreteCommand(GetCreateEdgeTableQuery(2), connection);
            createTable.ExecuteNonQuery();
            createTable = CreateConcreteCommand(GetCreateEdgeTableQuery(3), connection);
            createTable.ExecuteNonQuery();

            connection.Close();
        }

        public void PopulateDatabase(IGraph data)
        {
            connection.Open();

            HashSet<IEdge> edgeSet = new HashSet<IEdge>();
            HashSet<IVertex> vertexSet = new HashSet<IVertex>();

            foreach (GraphObjectTypeInfo t in data.RegisteredTypes)
            {
                int isVertex = BoolToInteger(t.Type == GraphObjectType.Vertex);
                string sql = $"INSERT INTO Type (id, name, description, is_vertex) VALUES ('{t.Id}', '{t.Description}', '{t.Name}', {isVertex})";
                DbCommand cmd = CreateConcreteCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }

            using (DbCommand cmd = CreateConcreteCommand(connection))
            {
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    foreach (IVertex v in data)
                    {
                        cmd.CommandText = $"INSERT INTO Vertex (id, type_id, content) VALUES ('{v.ObjectId}', '{v.TypeIdentity.Id}', '{v}')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = $"INSERT INTO IdType (id, lookup_table) VALUES ('{v.ObjectId}', 'Vertex')";
                        foreach (IEdge edge in v.GetLinkedEdges())
                        {
                            if (!edgeSet.Contains(edge))
                            {
                                edgeSet.Add(edge);
                            }
                        }
                    }
                    transaction.Commit();
                }

                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    foreach (IEdge e in edgeSet)
                    {
                        string table = GetLookupTable(e.TypeIdentity.Id);
                        StringBuilder query = new StringBuilder($"INSERT INTO {table} (");
                        List<IVertex> linkedVertices = e.GetLinkedNodes().ToList();

                        for (int i = 1; i <= linkedVertices.Count; i++)
                        {
                            query.Append($"item{i},");
                        }
                        query.Append("id, type_id) VALUES (");

                        foreach (IVertex v in linkedVertices)
                        {
                            query.Append($"'{v.ObjectId}', ");
                        }
                        query.Append($"'{e.ObjectId}', '{e.TypeIdentity.Id}')");

                        cmd.CommandText = query.ToString();
                        Debug.WriteLine(cmd.CommandText);

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = $"INSERT INTO IdType (id, lookup_table) VALUES ('{e.ObjectId}', '{table}')";
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }

            connection.Close();
        }

        public void RegisterTable(string tableName, Guid type)
        {
            lookupTables.AddIfNotExists(type, tableName);
        }

        private string GetCreateEdgeTableQuery(int n)
        {
            if (n < 2)
            {
                throw new ArgumentException("Parameter 'n' needs to be equals or higher to 2.");
            }

            StringBuilder query = new StringBuilder();
            query.Append($"CREATE TABLE Edge{n} (id TEXT NOT NULL PRIMARY KEY, type_id TEXT,");

            for (int i = 1; i <= n; i++)
            {
                query.Append($"item{i} TEXT NOT NULL,");
            }

            for (int i = 1; i <= n; i++)
            {
                query.Append($"FOREIGN KEY(item{i}) REFERENCES Vertex(id),");
            }

            query.Append("FOREIGN KEY(type_id) REFERENCES Type(id))");
            Debug.WriteLine(query.ToString());

            return query.ToString();
        }

        private void InsertLookupTableValue(IGraphObject obj, string value)
        {
        }

        private string GetLookupTable(Guid id)
        {
            if (lookupTables.ContainsKey(id))
            {
                return lookupTables[id];
            }
            throw new KeyNotFoundException($"Key '{id}' not found.");
        }

        private static int BoolToInteger(bool b)
        {
            return b ? 1 : 0;
        }

        private ConcreteDbCommand CreateConcreteCommand(string rawSql, DbConnection connection)
        {
            return (ConcreteDbCommand)Activator.CreateInstance(typeof(ConcreteDbCommand), new object[] { rawSql, connection });
        }

        private ConcreteDbCommand CreateConcreteCommand(DbConnection connection)
        {
            return (ConcreteDbCommand)Activator.CreateInstance(typeof(ConcreteDbCommand), new object[] { connection });
        }
    }
}
