namespace Leger.Extra.SqlBinding
{
	public class SqliteEdgeStoreRawSqlProvider : IEdgeStoreRawSqlProvider
	{
		public string SelectById => "SELECT * FROM {0} WHERE id = '{1}'";
	}
}