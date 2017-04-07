using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbTransactionProvider
    {
        void BeginTransaction();
        void BeginTransaction(IsolationLevel level);
        void Commit();
        void Rollback();
    }
}
