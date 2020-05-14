using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using System.Linq;
using Auditing.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Internal;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading;

namespace Auditing.Infrastructure.Repository
{
    public class DapperRepository : IRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDbConnection _connection;

        public DapperRepository(IDbConnection connection, IUnitOfWork unitOfWork)
        {
            _connection = connection;
            _unitOfWork = unitOfWork;
        }

        public DapperRepository(DbContext dbContext)
        {
            _connection = dbContext.Database.GetDbConnection();
        }

        public void Delete<TEntity>(params TEntity[] entities) where TEntity : class
        {
            entities.ToList().ForEach(entity => _connection.Delete<TEntity>(entity, _unitOfWork.Transaction));
        }

        public void Delete<TEntity>(params object[] ids) where TEntity : class
        {
            foreach (var id in ids)
            {
                var entity = _connection.Get<TEntity>(id);
                _connection.Delete<TEntity>(entity, _unitOfWork.Transaction);
            }
        }

        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return _connection.GetAll<TEntity>();
        }

        public TEntity GetByID<TEntity>(object id) where TEntity : class
        {
            return _connection.Get<TEntity>(id);
        }

        public TEntity GetByKeys<TEntity>(object keys) where TEntity : class
        {
            var tableName = typeof(TEntity).Name;
            var tableNameAttr = typeof(TEntity).GetCustomAttribute<TableAttribute>(false);
            if (tableNameAttr != null)
                tableName = tableNameAttr.Name;
            var expsWhere = string.Join(" AND ", keys.GetType().GetProperties().Select(x => $"{x} = @{x}"));
            return _connection.QueryFirst<TEntity>($"SELECT * FROM {tableName} WHERE {expsWhere}", keys);
        }

        public void Insert<TEntity>(params TEntity[] entities) where TEntity : class
        {
            entities.ToList().ForEach(entity => _connection.Insert(entity, _unitOfWork.Transaction));
        }

        public TEntity QueryFirst<TEntity>(string sql, object param) where TEntity : class
        {
            return _connection.QueryFirst<TEntity>(sql, param);
        }

        public TEntity QuerySingle<TEntity>(string sql, object param) where TEntity : class
        {
            return _connection.QuerySingle<TEntity>(sql, param);
        }

        public void Update<TEntity>(params TEntity[] entities) where TEntity : class
        {
            entities.ToList().ForEach(entity => _connection.Update(entity, _unitOfWork.Transaction));
        }

        public IEnumerable<TEntity> GetByQuery<TEntity>(Expression<Func<TEntity, bool>> exps) where TEntity : class
        {
            return _connection.GetAll<TEntity>().Where(exps.Compile()).ToList();
        }

        public IEnumerable<TEntity> GetByQuery<TEntity>(string sql, object param) where TEntity : class
        {
            return _connection.Query<TEntity>(sql, param);
        }

        public IEnumerable<TEntity> GetByQuery<TEntity>(SearchParameters searchParameters) where TEntity : class
        {
            var builder = new StringBuilder($"SELECT * FROM {typeof(TEntity).Name}");

            //构建Where语句
            var sqlWhere = searchParameters.BuildSqlWhere();
            if (!string.IsNullOrEmpty(sqlWhere.Item1))
                builder.Append(sqlWhere.Item1);

            //构建Order By语句
            var sqlOrderBy = searchParameters.BuildSqlOrderBy();
            if (!string.IsNullOrEmpty(sqlOrderBy))
                builder.Append(sqlOrderBy);

            //构建Limit语句
            var sqlLimit = searchParameters.BuildSqlLimit();
            if (!string.IsNullOrEmpty(sqlLimit))
                builder.Append(sqlLimit);

            //查询
            var sql = builder.ToString();
            if (sqlWhere.Item2 != null && sqlWhere.Item2.Any())
                return _connection.Query<TEntity>(sql, sqlWhere.Item2);
            return _connection.Query<TEntity>(sql);
        }
    }
}
