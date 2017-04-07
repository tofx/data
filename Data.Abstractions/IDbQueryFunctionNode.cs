using System.Collections.Generic;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public interface IDbQueryFunctionNode
    {
        bool CheckForHandle(string functionName);
        string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode);
    }
}
