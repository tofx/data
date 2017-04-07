using System.Collections.Generic;
using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryValueIncreDecreStrategy
    {
        bool SupportIncrease();
        bool SupportDecrease();
        string GetDbValueIncreaseQuery();
        string GetDbValueDecreaseQuery();
        IEnumerable<IDbDataParameter> GetDbIncreaseQueryParameters();
        IEnumerable<IDbDataParameter> GetDbDecreaseQueryParameters();

    }
}
