using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Auditing.Infrastructure
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// IDbConnection
        /// </summary>
        private readonly IDbConnection _connection = null;
        public IDbConnection Connection => _connection;

        /// <summary>
        /// IDbTransaction
        /// </summary>
        private IDbTransaction _transaction = null;
        public IDbTransaction Transaction
        {
            get
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                if (_transaction == null)
                    _transaction = _connection.BeginTransaction();

                return _transaction;
            }
        }

        /// <summary>
        /// ILogger<DapperUnitOfWork>
        /// </summary>
        private ILogger<DapperUnitOfWork> _logger;

        public DapperUnitOfWork(IDbConnection connection, ILogger<DapperUnitOfWork> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DapperUnitOfWork)} commit fails", new object[] { _connection.ConnectionString, _connection.State });
                _transaction.Rollback();
            }
            finally
            {
                _connection.Close();
                _connection.Dispose();
            }

        }
    }
}
