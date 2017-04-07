using System.Collections.Generic;
using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbQueryParameterParsingStrategy
    {
        IEnumerable<IDbDataParameter> Parse();
    }
}
