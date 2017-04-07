using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.DateTime
{
    public class SqlDbQueryFunctionDateTimeHourNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Hour", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(functionExpressionNode, ModelStrategy);

            expressionBuilder.Append("DATEPART(HOUR, ");
            expressionBuilder.Append(expParser.Parse(parameterDictionary, functionExpressionNode));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
