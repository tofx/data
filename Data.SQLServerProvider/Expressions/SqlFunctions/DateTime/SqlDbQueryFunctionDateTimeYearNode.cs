using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.DateTime
{
    public class SqlDbQueryFunctionDateTimeYearNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Year", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(functionExpressionNode, ModelStrategy);

            expressionBuilder.Append("DATEPART(YEAR, ");
            expressionBuilder.Append(expParser.Parse(parameterDictionary, functionExpressionNode));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
