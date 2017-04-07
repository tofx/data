using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryParameterParsingStrategyFactory
    {
        IDbQueryParameterParsingStrategy GetParsingStrategy(object value = null);
    }
}
