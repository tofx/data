using System.Collections.Generic;
using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbEnumTableInspectorStrategy
    {
        string GetDbQueryScript();
        IEnumerable<IDbDataParameter> GetDbParameters();
    }
}
