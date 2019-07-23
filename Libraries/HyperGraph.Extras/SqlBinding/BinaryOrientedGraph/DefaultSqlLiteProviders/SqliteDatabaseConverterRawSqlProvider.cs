namespace Leger.Extra.SqlBinding
{
	public class SqliteDatabaseConverterRawSqlProvider : IDatabaseConverterRawSqlProvider
	{
		public string CreateTypeTable => "CREATE TABLE Type(name TEXT, id TEXT NOT NULL PRIMARY KEY, is_vertex BOOLEAN)";

		public string CreateVertexTable => "CREATE TABLE Vertex(id TEXT NOT NULL PRIMARY KEY, type_id TEXT, content TEXT, FOREIGN KEY(type_id) REFERENCES Type(id))";

		public string CreateEdgeTable => @"CREATE TABLE Edge (id TEXT NOT NULL PRIMARY KEY, type_id TEXT, source TEXT, target TEXT,
	                FOREIGN KEY(source) REFERENCES Vertex(id), 
	                FOREIGN KEY(target) REFERENCES Vertex(id),
	                FOREIGN KEY(type_id) REFERENCES Type(id))";

		public string InsertType => "INSERT INTO Type (id, name, is_vertex) VALUES('{0}', '{1}', {2})";

		public string InsertVertex => "INSERT INTO Vertex (id, type_id, content) VALUES('{0}', '{1}', '{2}')";

		public string InsertEdge => "INSERT INTO Edge (id, type_id, source, target) VALUES ('{0}', '{1}', '{2}', '{3}')";
	}
}
