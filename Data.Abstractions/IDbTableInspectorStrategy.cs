using System.Collections.Generic;
using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbTableInspectorStrategy
    {
        string GetDbQueryScript();
        IEnumerable<IDbDataParameter> GetDbParameters();
    }
}
