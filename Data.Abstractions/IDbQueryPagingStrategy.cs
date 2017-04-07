namespace tofx.Data.Abstractions
{
    public interface IDbQueryPagingStrategy
    {
        void DetectQueryRowCount();
        void MoveFirst();
        void MoveLast();
        void MoveNext();
        void MovePrevious();
    }
}
