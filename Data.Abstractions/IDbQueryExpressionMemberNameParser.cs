using System.Collections.Generic;
using System.Linq.Expressions;

namespace TOF.Data.Abstractions
{
    public interface IDbQueryExpressionMemberNameParser
    {
        string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode);
    }
}
