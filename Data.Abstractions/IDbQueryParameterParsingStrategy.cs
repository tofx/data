using System.Collections.Generic;
using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryParameterParsingStrategy
    {
        IEnumerable<IDbDataParameter> Parse();
    }
}
