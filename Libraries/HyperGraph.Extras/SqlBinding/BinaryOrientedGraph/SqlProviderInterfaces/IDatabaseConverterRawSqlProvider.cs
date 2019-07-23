namespace Leger.Extra.SqlBinding
{
	public interface IDatabaseConverterRawSqlProvider
	{
		string CreateTypeTable { get; }
		string CreateVertexTable { get; }
		string CreateEdgeTable { get; }
		string InsertType { get; }
		string InsertVertex { get; }
		string InsertEdge { get; }
	}
}