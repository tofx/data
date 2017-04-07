using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.Common
{
    public class SqlDbQueryFunctionEqualsNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Equals", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp, ModelStrategy);
            var callSourceExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Object, ModelStrategy);
            var argExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), ModelStrategy);

            expressionBuilder.Append("(");
            expressionBuilder.Append(callSourceExp.Parse(parameterDictionary, exp.Object));
            expressionBuilder.Append(" = ");
            expressionBuilder.Append(argExp.Parse(parameterDictionary, exp.Arguments.First()));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
