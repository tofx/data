using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionIsNullOrEmptyNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("IsNullOrEmpty", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp, ModelStrategy);
            var argExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), ModelStrategy);

            expressionBuilder.Append("(");
            expressionBuilder.Append(argExp.Parse(parameterDictionary, exp.Arguments.First()));
            expressionBuilder.Append(" IS NULL) OR (LEN(");
            expressionBuilder.Append(argExp.Parse(parameterDictionary, exp.Arguments.First()));
            expressionBuilder.Append(") = 0)");

            return expressionBuilder.ToString();
        }
    }
}
