using System.Collections.Generic;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryExpressionParser
    {
        string Parse(IDictionary<string, object> parameterDictionary, Expression queryExpressionRoot, IDbModelStrategy modelStrategy);
    }
}
