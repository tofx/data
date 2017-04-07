using System.Collections.Generic;
using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbQueryStrategy
    {
        string GetDbQueryScript();
        IEnumerable<IDbDataParameter> GetDbParameters();
    }
}