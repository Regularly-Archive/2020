using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

namespace Auditing.Infrastructure.Repository
{
    public class SearchParameters
    {
        public PageInfo PageInfo { get; set; }
        public List<Condition> ConditionItems { get; set; }
    }

    public class PageInfo
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 500;
        public SortType SortType { get; set; }
        public List<string> SortFields { get; set; }
    }

    public enum SortType
    {
        Asc = 10,
        Desc = 20
    }

    public enum Operator
    {
        Equals = 10,
        NotEquals = 20,
        LessThan = 30,
        LessThanOrEquals = 40,
        GreaterThen = 50,
        GreaterThenOrEquals = 60,
        StdIn = 70,
        NotStdIn = 80,
        Contains = 90,
        NotContains = 100,
        StartsWith = 110,
        EndsWith = 120
    }

    public class Condition
    {
        public string Field { get; set; }
        public Operator Op { get; set; }
        public object Value { get; set; }
        public string OrGroup { get; set; }
    }

    public static class SearchParametersExtension
    {
        public static (string, Dictionary<string, object>) BuildSqlWhere(this SearchParameters searchParameters)
        {
            var conditions = searchParameters.ConditionItems;
            if (conditions == null || !conditions.Any())
                return (string.Empty, null);

            var sqlExps = new List<string>();
            var sqlParam = new Dictionary<string, object>();

            //构建简单条件
            var simpleConditions = conditions.FindAll(x => string.IsNullOrEmpty(x.OrGroup));
            sqlExps.Add(simpleConditions.BuildSqlWhere(ref sqlParam));

            //构建复杂条件
            var complexConditions = conditions.FindAll(x => !string.IsNullOrEmpty(x.OrGroup));
            sqlExps.AddRange(complexConditions.GroupBy(x => x.OrGroup).ToList().Select(x => "( " + x.BuildSqlWhere(ref sqlParam," OR ") + " )"));

            var sqlWhwere = sqlExps.Count > 1 ? sqlExps.Join(" AND ") : sqlExps[0];
            return ($" WHERE {sqlWhwere} ", sqlParam);
        }

        public static string BuildSqlOrderBy(this SearchParameters searchParameters)
        {
            var pageInfo = searchParameters.PageInfo;
            if (pageInfo.SortFields != null && pageInfo.SortFields.Any())
            {
                var orderBy = pageInfo.SortFields.Join(",");
                var orderType = pageInfo.SortType == SortType.Asc ? "ASC" : "DESC";
                return $" ORDER BY {orderBy} {orderType}";
            }

            return string.Empty;
        }

        public static string BuildSqlLimit(this SearchParameters searchParameters)
        {
            var pageInfo = searchParameters.PageInfo;
            if (pageInfo.SortFields != null && pageInfo.SortFields.Any())
            {
                var skipCount = (pageInfo.CurrentPage - 1) * pageInfo.PageSize;
                var pageSize = pageInfo.PageSize;
                return ($" LIMIT {skipCount},{pageSize}");
            }

            return string.Empty;
        }


        public static string BuildSqlWhere(this IEnumerable<Condition> conditions, ref Dictionary<string, object> sqlParams, string keywords = " AND ")
        {
            if (conditions == null || !conditions.Any())
                return string.Empty;

            var sqlParamIndex = 1;
            var sqlExps = new List<string>();
            foreach (var condition in conditions)
            {
                var index = sqlParams.Count + sqlParamIndex;
                switch (condition.Op)
                {
                    case Operator.Equals:
                        sqlExps.Add($"{condition.Field} = @Param{index}");
                        sqlParams[$"Param{index}"] = condition.Value;
                        break;
                    case Operator.NotEquals:
                        sqlExps.Add($"{condition.Field} <> @Param{index}");
                        sqlParams[$"Param{index}"] = condition.Value;
                        break;
                    case Operator.Contains:
                        sqlExps.Add($"{condition.Field} LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"%{condition.Value}%";
                        break;
                    case Operator.NotContains:
                        sqlExps.Add($"{condition.Field} NOT LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"%{condition.Value}%";
                        break;
                    case Operator.StartsWith:
                        sqlExps.Add($"{condition.Field} LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"%{condition.Value}";
                        break;
                    case Operator.EndsWith:
                        sqlExps.Add($"{condition.Field} LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}%";
                        break;
                    case Operator.GreaterThen:
                        sqlExps.Add($"{condition.Field} > @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operator.GreaterThenOrEquals:
                        sqlExps.Add($"{condition.Field} >= @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operator.LessThan:
                        sqlExps.Add($"{condition.Field} < @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operator.LessThanOrEquals:
                        sqlExps.Add($"{condition.Field} <= @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operator.StdIn:
                        sqlExps.Add($"{condition.Field} IN @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operator.NotStdIn:
                        sqlExps.Add($"{condition.Field} NOT IN @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                }

                sqlParamIndex += 1;
            }

            return sqlExps.Count > 1 ? sqlExps.Join(keywords) : sqlExps[0];
        }

    }
}
