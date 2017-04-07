using System.Collections.Generic;
using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryStrategy
    {
        string GetDbQueryScript();
        IEnumerable<IDbDataParameter> GetDbParameters();
    }
}