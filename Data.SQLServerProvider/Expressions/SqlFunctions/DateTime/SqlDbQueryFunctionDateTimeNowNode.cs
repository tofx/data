using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.DateTime
{
    public class SqlDbQueryFunctionDateTimeNowNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Now", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            return expressionBuilder.Append("GETDATE()").ToString();
        }
    }
}
