using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbTransactionProvider
    {
        void BeginTransaction();
        void BeginTransaction(IsolationLevel level);
        void Commit();
        void Rollback();
    }
}
