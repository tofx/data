using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionLengthNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {   
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Length", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(functionExpressionNode, ModelStrategy);

            expressionBuilder.Append("LEN(");
            expressionBuilder.Append(expParser.Parse(parameterDictionary, functionExpressionNode));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
