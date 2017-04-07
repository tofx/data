using System.Collections.Generic;
using System.Data;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.ParameterParsingStrategies
{
    public class NullParameterParsingStrategy : IDbQueryParameterParsingStrategy
    {
        public IEnumerable<IDbDataParameter> Parse()
        {
            return new List<IDbDataParameter>();
        }
    }
}
