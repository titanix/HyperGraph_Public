namespace Leger.Extra.SqlBinding
{
	public class SqliteVertexStoreRawSqlProvider : IVertexStoreRawSqlProvider
	{
		public string SelectVertexAndPredecessors => @"
                    SELECT
                        Vertex.id AS origin_vertex_id,
	                    Vertex.type_id AS origin_vertex_type_info,
	                    Vertex.content AS origin_vertex_content,
	                    E1.id AS edge_id,
	                    E1.type_id AS edge_type_info,
	                    E1.target AS target_vertex_id,
	                    V2.content AS target_vertex_content,
	                    V2.id AS target_vertex_id,
	                    V2.type_id AS target_vertex_type_info
                    FROM Vertex
                    INNER JOIN Edge AS E1 ON Vertex.id = E1.source
                    INNER JOIN VERTEX AS V2 ON E1.target = V2.id
                    WHERE Vertex.id = '{0}'";

		public string SelectVertexAndSuccessors => @"
                    SELECT
                        Vertex.id AS origin_vertex_id,
	                    Vertex.type_id AS origin_vertex_type_info,
	                    Vertex.content AS origin_vertex_content,
	                    E1.id AS edge_id,
	                    E1.type_id AS edge_type_info,
	                    E1.source AS target_vertex_id,
	                    V2.content AS target_vertex_content,
	                    V2.id AS target_vertex_id,
	                    V2.type_id AS target_vertex_type_info
                    FROM Vertex
                    INNER JOIN Edge AS E1 ON Vertex.id = E1.target
                    INNER JOIN VERTEX AS V2 ON E1.source = V2.id
                    WHERE Vertex.id = '{0}'";

		public string SelectByContent => "SELECT id FROM {0} WHERE content = '{1}'";
	}
}