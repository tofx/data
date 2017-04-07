using System.Collections.Generic;
using System.Linq.Expressions;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions
{
    public abstract class SqlDbQueryExpressionNode : IDbQueryExpressionNode
    {
        public IDbModelStrategy ModelStrategy { get; set; }
        public abstract string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode);
    }
}
