using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Leger.Extra.SqlBinding
{
    public class OrientedBinaryGraphToDatabaseConverter<ConcreteDbCommand> : IDatabaseCreator where ConcreteDbCommand : DbCommand, new()
    {
        private DbConnection connection;
		private IDatabaseConverterRawSqlProvider sqlQueriesProvider;

		public OrientedBinaryGraphToDatabaseConverter(DbConnection connection)
		{
			this.connection = connection;
			sqlQueriesProvider = new SqliteDatabaseConverterRawSqlProvider();
		}

		public OrientedBinaryGraphToDatabaseConverter(DbConnection connection, IDatabaseConverterRawSqlProvider sqlQueries)
		{
			this.connection = connection;
            this.sqlQueriesProvider = sqlQueries;
		}

        public void CreateDatabase()
        {
            connection.Open();

			connection.ExecuteNonQuery<ConcreteDbCommand>(sqlQueriesProvider.CreateTypeTable);

			connection.ExecuteNonQuery<ConcreteDbCommand>(sqlQueriesProvider.CreateVertexTable);

			connection.ExecuteNonQuery<ConcreteDbCommand>(sqlQueriesProvider.CreateEdgeTable);

            connection.Close();
        }

        public void PopulateDatabase(IGraph data)
        {
            connection.Open();

            HashSet<IEdge> edgeSet = new HashSet<IEdge>();

            foreach (GraphObjectTypeInfo t in data.RegisteredTypes)
            {
                int isVertex = BoolToInteger(t.Type == GraphObjectType.Vertex);
				connection.ExecuteNonQuery<ConcreteDbCommand>(String.Format(sqlQueriesProvider.InsertType, t.Id, t.Name, isVertex));
            }

            using (DbCommand cmd = (ConcreteDbCommand)Activator.CreateInstance(typeof(ConcreteDbCommand)))
            {
                cmd.Connection = connection;
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    foreach (IVertex v in data)
                    {
                        cmd.CommandText = String.Format(sqlQueriesProvider.InsertVertex, v.ObjectId, v.TypeIdentity.Id, v);
                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
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
                    cmd.Transaction = transaction;
                    foreach (IEdge e in edgeSet)
                    {
                        cmd.CommandText = String.Format(sqlQueriesProvider.InsertEdge, e.ObjectId, e.TypeIdentity.Id, e.GetLinkedObjects()[0].ObjectId, e.GetLinkedObjects()[1].ObjectId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }

            connection.Close();
        }

        private static int BoolToInteger(bool b)
        {
            return b ? 1 : 0;
        }
    }
}
