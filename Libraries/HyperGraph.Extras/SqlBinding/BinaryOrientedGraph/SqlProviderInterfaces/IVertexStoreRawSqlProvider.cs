namespace Leger.Extra.SqlBinding
{
	public interface IVertexStoreRawSqlProvider
	{
		string SelectVertexAndPredecessors { get; }
		string SelectVertexAndSuccessors { get; }
		string SelectByContent { get; }
	}
}