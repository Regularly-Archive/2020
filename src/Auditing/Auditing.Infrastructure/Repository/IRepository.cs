using Auditing.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Auditing.Infrastructure.Repository
{
    public interface  IRepository
    {
        TEntity GetByID<TEntity>(object id) where TEntity : class;
        TEntity GetByKeys<TEntity>(object keys) where TEntity : class;
        TEntity QueryFirst<TEntity>(string sql, object param) where TEntity : class;
        TEntity QuerySingle<TEntity>(string sql, object param) where TEntity : class;
        [AuditLog(OperationType.Created)]
        void Insert<TEntity>(params TEntity[] entities) where TEntity : class;
        [AuditLog(OperationType.Deleted)]
        void Update<TEntity>(params TEntity[] entities) where TEntity : class;
        [AuditLog(OperationType.Deleted)]
        void Delete<TEntity>(params TEntity[] entities) where TEntity : class;
        void Delete<TEntity>(params object[] ids) where TEntity : class;
        IEnumerable<TEntity> GetByQuery<TEntity>(Expression<Func<TEntity,bool>> exps) where TEntity : class;
        IEnumerable<TEntity> GetByQuery<TEntity>(string sql, object param) where TEntity : class;
        IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;
    }
}
