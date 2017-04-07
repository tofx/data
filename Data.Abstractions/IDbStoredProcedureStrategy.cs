using System.Collections.Generic;
using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbStoredProcedureStrategy
    {
        string GetDbProcName();
        IEnumerable<IDbDataParameter> GetDbParameters();
        IDictionary<string, IDbModelPropertyStrategy> GetModelPropertyBindings();
    }
}
