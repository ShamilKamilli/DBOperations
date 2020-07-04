using DBOperations.src.Exceptions;
using DBOperations.src.Mapping;
using DBOperations.src.Mapping.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DBOperations.src.DB
{
    public abstract class DbContext:IDisposable
    {
        private DbConnection _dbConnection = null;
        private DbTransaction _dbTransaction = null;
    
        public DbContext(string connectionString)
        {
            _dbConnection = InitializeConnection();
            _dbConnection.ConnectionString = connectionString;
            _dbConnection.Open();
        }

        public abstract DbConnection InitializeConnection();

        protected void ExecuteCommand(DbCommand dbCommand)
        {
            dbCommand.Connection = _dbConnection;

            if (_dbTransaction != null)
                dbCommand.Transaction = _dbTransaction;

            using (dbCommand)
            {
                dbCommand.ExecuteNonQuery();
            }
        }

        protected async Task ExecuteCommandAsync(DbCommand dbCommand)
        {
            dbCommand.Connection = _dbConnection;

            if (_dbTransaction != null)
                dbCommand.Transaction = _dbTransaction;

            using (dbCommand)
            {
               await dbCommand.ExecuteNonQueryAsync();
            }
        }

        protected IEnumerable<T> Get<T>(DbCommand dbCommand)
        {

            dbCommand.Connection = _dbConnection;

            if (_dbTransaction != null)
                dbCommand.Transaction = _dbTransaction;

            using (dbCommand)
            {
                var reader = dbCommand.ExecuteReader();

                var configMembers = QueryConfiguration.GetMembers(typeof(T));

                if (configMembers == null)
                    throw new MapperMemberCannotFindException($"type cannot find {typeof(T).Name}");

                var serializer = new Serializer<T>(configMembers);

                return serializer.Constructs(reader);
            }
        }

        protected async Task<IEnumerable<T>> GetAsync<T>(DbCommand dbCommand)
        {

            dbCommand.Connection = _dbConnection;

            if (_dbTransaction != null)
                dbCommand.Transaction = _dbTransaction;

            using (dbCommand)
            {
                using (var reader = await dbCommand.ExecuteReaderAsync())
                {
                    var configMembers = QueryConfiguration.GetMembers(typeof(T));

                    if (configMembers == null)
                        throw new MapperMemberCannotFindException($"Type cannot find {typeof(T).Name}");

                    var serializer = new Serializer<T>(configMembers);

                    return serializer.Constructs(reader);
                }
            }
        }

        public void BeginTransaction()
        {
            if (_dbTransaction != null)
                throw new InvalidOperationException("Threre is uncommited transaction");

            _dbTransaction = _dbConnection.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_dbTransaction != null)
                throw new InvalidOperationException("There is uncommited transaction");

            _dbTransaction = _dbConnection.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction()
        {
            if (_dbTransaction == null)
                throw new InvalidOperationException("There is not active transaction");

            _dbTransaction.Commit();
            _dbTransaction.Dispose();
            _dbTransaction = null;
        }

        public void RollbackTransaction()
        {
            if (_dbTransaction == null)
                throw new InvalidOperationException("There is not active transaction");

            _dbTransaction.Rollback();
            _dbTransaction.Dispose();
            _dbTransaction = null;
        }

        public void Dispose()
        {
            _dbConnection?.Dispose();
        }
    }
}
