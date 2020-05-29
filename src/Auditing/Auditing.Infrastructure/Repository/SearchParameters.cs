using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace Auditing.Infrastructure.Repository
{
    public class SearchParameters
    {
        public PageModel Page { get; set; }
        public QueryModel Query { get; set; }
    }

    public class QueryModel : List<Condition>
    {
        public void Add<T>(Condition<T> condition) where T : class
        {
            var filedName = string.Empty;
            var memberExp = condition.Field.Body as MemberExpression;
            if (memberExp == null)
            {
                var ubody = (UnaryExpression)condition.Field.Body;
                memberExp = ubody.Operand as MemberExpression;
            }
            filedName = memberExp.Member.Name;
            Add(new Condition() { Field = filedName, Op = condition.Op, Value = condition.Value, OrGroup = condition.OrGroup });
        }
    }

    public class PageModel
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

    public enum Operation
    {
        Equals = 10,
        NotEquals = 20,
        LessThan = 30,
        LessThanOrEquals = 40,
        GreaterThen = 50,
        GreaterThenOrEquals = 60,
        StdIn = 70,
        StdNotIn = 80,
        Contains = 90,
        NotContains = 100,
        StartsWith = 110,
        EndsWith = 120
    }

    public class Condition
    {
        public string Field { get; set; }
        public Operation Op { get; set; }
        public object Value { get; set; }
        public string OrGroup { get; set; }
    }

    public class Condition<T> : Condition
    {
        public new Expression<Func<T, dynamic>> Field { get; set; }
        public Operation Op { get; set; }
        public object Value { get; set; }
        public string OrGroup { get; set; }
    }

    public static class SearchParametersExtension
    {
        public static (string, Dictionary<string, object>) BuildSqlWhere(this SearchParameters searchParameters)
        {
            var conditions = searchParameters.Query;
            if (conditions == null || !conditions.Any())
                return (string.Empty, null);

            var sqlExps = new List<string>();
            var sqlParam = new Dictionary<string, object>();

            //构建简单条件
            var simpleConditions = conditions.FindAll(x => string.IsNullOrEmpty(x.OrGroup));
            sqlExps.Add(simpleConditions.BuildSqlWhere(ref sqlParam));

            //构建复杂条件
            var complexConditions = conditions.FindAll(x => !string.IsNullOrEmpty(x.OrGroup));
            sqlExps.AddRange(complexConditions.GroupBy(x => x.OrGroup).ToList().Select(x => "( " + x.BuildSqlWhere(ref sqlParam, " OR ") + " )"));

            var sqlWhwere = sqlExps.Count > 1 ? string.Join(" AND ", sqlExps) : sqlExps[0];
            return ($" WHERE {sqlWhwere} ", sqlParam);
        }

        public static string BuildSqlOrderBy(this SearchParameters searchParameters)
        {
            var pageInfo = searchParameters.Page;
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
            var pageInfo = searchParameters.Page;
            var skipCount = (pageInfo.CurrentPage - 1) * pageInfo.PageSize;
            var pageSize = pageInfo.PageSize;
            return ($" LIMIT {skipCount},{pageSize}");
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
                    case Operation.Equals:
                        sqlExps.Add($"{condition.Field} = @Param{index}");
                        sqlParams[$"Param{index}"] = condition.Value;
                        break;
                    case Operation.NotEquals:
                        sqlExps.Add($"{condition.Field} <> @Param{index}");
                        sqlParams[$"Param{index}"] = condition.Value;
                        break;
                    case Operation.Contains:
                        sqlExps.Add($"{condition.Field} LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"%{condition.Value}%";
                        break;
                    case Operation.NotContains:
                        sqlExps.Add($"{condition.Field} NOT LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"%{condition.Value}%";
                        break;
                    case Operation.StartsWith:
                        sqlExps.Add($"{condition.Field} LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"%{condition.Value}";
                        break;
                    case Operation.EndsWith:
                        sqlExps.Add($"{condition.Field} LIKE @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}%";
                        break;
                    case Operation.GreaterThen:
                        sqlExps.Add($"{condition.Field} > @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operation.GreaterThenOrEquals:
                        sqlExps.Add($"{condition.Field} >= @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operation.LessThan:
                        sqlExps.Add($"{condition.Field} < @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operation.LessThanOrEquals:
                        sqlExps.Add($"{condition.Field} <= @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operation.StdIn:
                        sqlExps.Add($"{condition.Field} IN @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                    case Operation.StdNotIn:
                        sqlExps.Add($"{condition.Field} NOT IN @Param{index}");
                        sqlParams[$"Param{index}"] = $"{condition.Value}";
                        break;
                }

                sqlParamIndex += 1;
            }

            return sqlExps.Count > 1 ? string.Join(keywords, sqlExps) : sqlExps[0];
        }

        public static Func<T, bool> BuildWhere<T>(this IEnumerable<Condition> conditions)
        {
            var lambda = LambdaExpressionBuilder.BuildLambda<T>(conditions);
            return lambda.Compile();
        }
    }
}
