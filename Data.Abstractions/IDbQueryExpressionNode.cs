using System.Collections.Generic;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryExpressionNode
    {
        string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode);
    }
}
