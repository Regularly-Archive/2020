using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Dapper.Contrib.SqlAdapters
{
    public static class SqlAdapterrExtensions
    {
        public static void UseSqlAdapter<TSqlAdapter>(this IDbConnection connection, TSqlAdapter sqlAdapter)
            where TSqlAdapter : ISqlAdapter, new()
        {
            var adapters = (Dictionary<string, ISqlAdapter>)
                    typeof(SqlMapperExtensions)
                    .GetField("AdapterDictionary", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    ?.GetValue(null);

            var connectionType = connection.GetType().Name.ToLower();
            if (adapters != null && !adapters.ContainsKey(connectionType))
                adapters?.Add(connectionType, sqlAdapter);
        }
    }
}
