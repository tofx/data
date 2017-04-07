using System.Collections.Generic;
using System.Data;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.ParameterParsingStrategies
{
    public class NullParameterParsingStrategy : IDbQueryParameterParsingStrategy
    {
        public IEnumerable<IDbDataParameter> Parse()
        {
            return new List<IDbDataParameter>();
        }
    }
}
