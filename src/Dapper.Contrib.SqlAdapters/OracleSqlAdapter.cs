using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.SqlAdapters
{
    public class OracleSqlAdapter : ISqlAdapter
    {
        public void AppendColumnName(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("{0}", columnName);
        }

        public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("{0} = :{1}", columnName, columnName);
        }

        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            parameterList = parameterList.Replace("@", ":");
            var sql = $"insert into {tableName} ({columnList}) values ({parameterList})";
            return connection.Execute(sql, entityToInsert, transaction, commandTimeout);
        }

        public Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            parameterList = parameterList.Replace("@", ":");
            var sql = $"insert into {tableName} ({columnList}) values ({parameterList})";
            return connection.ExecuteAsync(sql, entityToInsert, transaction, commandTimeout);
        }
    }
}
