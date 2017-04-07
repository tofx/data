using System.Collections.Generic;

namespace TOF.Data.Abstractions
{
    public interface IDbClient
    {
        void Open();
        void Close();
        int Execute(string sql, object dataParams = null);
        int Execute(string sql, object[] dataParamsArray = null);
        IEnumerable<dynamic> ExecuteQuery(string sql, object queryParams = null);
        IEnumerable<T> ExecuteQuery<T>(string sql, object queryParams = null) where T : class, new();
        object ExecuteScalar(string sql, object queryParams = null);
        T ExecuteScalar<T>(string sql, object queryParams = null);
    }
}
