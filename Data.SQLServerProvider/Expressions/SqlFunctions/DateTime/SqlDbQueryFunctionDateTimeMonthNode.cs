using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.DateTime
{
    public class SqlDbQueryFunctionDateTimeMonthNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Month", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(functionExpressionNode, ModelStrategy);

            expressionBuilder.Append("DATEPART(MONTH, ");
            expressionBuilder.Append(expParser.Parse(parameterDictionary, functionExpressionNode));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
