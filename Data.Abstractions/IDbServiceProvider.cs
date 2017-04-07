using System;
using System.Collections.Generic;
using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbServiceProvider : IDbTransactionProvider, IDisposable
    {
        void Open();
        void Close();
        bool IsConnectionOpen();
        ConnectionState GetCurrentConnectionState();
        int Execute(string statement, IEnumerable<IDbDataParameter> parameters);
        int RunStoredProc(string procedureName, IEnumerable<IDbDataParameter> parameters);
        IDataReader QueryGetReader(string statement, IEnumerable<IDbDataParameter> parameters);
        object QueryGetScalar(string statement, IEnumerable<IDbDataParameter> parameters);
        IDataReader RunStoredProcGetReader(string procedureName, IEnumerable<IDbDataParameter> parameters);
        object RunStoredProcGetScalar(string procedureName, IEnumerable<IDbDataParameter> parameters);
    }
}
