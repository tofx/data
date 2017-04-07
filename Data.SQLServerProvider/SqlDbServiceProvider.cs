using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer
{
    public class SqlDbServiceProvider : IDbServiceProvider
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transactionContext;

        public SqlDbServiceProvider(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public ConnectionState GetCurrentConnectionState()
        {
            return _connection.State;
        }

        public bool IsConnectionOpen()
        {
            return GetCurrentConnectionState() == ConnectionState.Open;
        }

        public void Open()
        {
            var state = GetCurrentConnectionState();

            if (state == ConnectionState.Closed)
                _connection.Open();
        }

        public void Close()
        {
            var state = GetCurrentConnectionState();

            if (state == ConnectionState.Open || state == ConnectionState.Connecting)
                _connection.Close();
        }

        public void BeginTransaction()
        {
            _transactionContext = _connection.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel level)
        {
            _transactionContext = _connection.BeginTransaction(level);
        }

        public void Commit()
        {
            _transactionContext.Commit();
        }

        public void Rollback()
        {
            _transactionContext.Rollback();
        }

        public int Execute(string statement, IEnumerable<IDbDataParameter> parameters)
        {
            SqlCommand command = new SqlCommand(statement, _connection);

            if (_transactionContext != null)
                command.Transaction = _transactionContext;

            foreach (var p in parameters)
                command.Parameters.Add(p as SqlParameter);

            return command.ExecuteNonQuery();
        }

        public IDataReader QueryGetReader(string statement, IEnumerable<IDbDataParameter> parameters)
        {
            SqlCommand command = new SqlCommand(statement, _connection);

            if (_transactionContext != null)
                command.Transaction = _transactionContext;

            if (parameters != null)
            {
                foreach (var p in parameters)
                    command.Parameters.Add(p as SqlParameter);
            }

            return command.ExecuteReader();
        }

        public object QueryGetScalar(string statement, IEnumerable<IDbDataParameter> parameters)
        {
            SqlCommand command = new SqlCommand(statement, _connection);

            if (_transactionContext != null)
                command.Transaction = _transactionContext;

            if (parameters != null)
            {
                foreach (var p in parameters)
                    command.Parameters.Add(p as SqlParameter);
            }

            return command.ExecuteScalar();
        }


        public int RunStoredProc(string procedureName, IEnumerable<IDbDataParameter> parameters)
        {
            SqlCommand command = new SqlCommand(procedureName, _connection);
            command.CommandType = CommandType.StoredProcedure;

            if (_transactionContext != null)
                command.Transaction = _transactionContext;

            foreach (var p in parameters)
                command.Parameters.Add(p as SqlParameter);

            return command.ExecuteNonQuery();
        }

        public IDataReader RunStoredProcGetReader(string procedureName, IEnumerable<IDbDataParameter> parameters)
        {
            SqlCommand command = new SqlCommand(procedureName, _connection);
            command.CommandType = CommandType.StoredProcedure;

            if (_transactionContext != null)
                command.Transaction = _transactionContext;

            if (parameters != null)
            {
                foreach (var p in parameters)
                    command.Parameters.Add(p as SqlParameter);
            }

            return command.ExecuteReader();
        }

        public object RunStoredProcGetScalar(string procedureName, IEnumerable<IDbDataParameter> parameters)
        {
            SqlCommand command = new SqlCommand(procedureName, _connection);
            command.CommandType = CommandType.StoredProcedure;

            if (_transactionContext != null)
                command.Transaction = _transactionContext;

            if (parameters != null)
            {
                foreach (var p in parameters)
                    command.Parameters.Add(p as SqlParameter);
            }

            return command.ExecuteScalar();
        }

        public void Dispose()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                if (_connection.State == ConnectionState.Open ||
                    _connection.State == ConnectionState.Broken ||
                    _connection.State == ConnectionState.Connecting)
                    _connection.Close();
                else
                {
                    while (
                        _connection.State == ConnectionState.Executing ||
                        _connection.State == ConnectionState.Fetching
                        )
                    {
                        Thread.Sleep(10);
                    }
                }
            }

            _connection.Dispose();
        }
    }
}
