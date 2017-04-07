using System.Collections.Generic;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryExpressionMemberNameParser
    {
        string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode);
    }
}
