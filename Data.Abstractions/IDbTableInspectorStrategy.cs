using System.Collections.Generic;
using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbTableInspectorStrategy
    {
        string GetDbQueryScript();
        IEnumerable<IDbDataParameter> GetDbParameters();
    }
}
