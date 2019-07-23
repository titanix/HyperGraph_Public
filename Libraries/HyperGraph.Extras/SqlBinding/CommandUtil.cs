using System;
using System.Data.Common;

namespace Leger.Extra.SqlBinding
{
    public static class CommandUtil
    {
        public static int ExecuteNonQuery<ConcreteDbCommand>(this DbConnection connection, string sql) where ConcreteDbCommand : DbCommand, new()
        {
            int result = 0;

            using (DbCommand command = (ConcreteDbCommand)Activator.CreateInstance(typeof(ConcreteDbCommand), new object[] { sql, connection }))
            {
                result = command.ExecuteNonQuery();
            }

            return result;
        }

        public static object ExecuteScalar<ConcreteDbCommand>(this DbConnection connection, string sql) where ConcreteDbCommand : DbCommand, new()
        {
            object result = null;

            using (DbCommand command = (ConcreteDbCommand)Activator.CreateInstance(typeof(ConcreteDbCommand), new object[] { sql, connection }))
            {
                result = command.ExecuteScalar();
            }

            return result;
        }
    }
}
