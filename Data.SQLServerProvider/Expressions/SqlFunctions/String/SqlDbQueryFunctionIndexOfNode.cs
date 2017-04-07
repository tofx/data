using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionIndexOfNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("IndexOf", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = (MethodCallExpression)functionExpressionNode;
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp, ModelStrategy);
            var argExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), ModelStrategy);
            var callSourceExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Object, ModelStrategy);
            var argument = argExp.Parse(parameterDictionary, exp.Arguments.First());

            expressionBuilder.Append("CHARINDEX(");
            expressionBuilder.Append(argExp.Parse(parameterDictionary, exp.Arguments.First()));
            expressionBuilder.Append(", ");
            expressionBuilder.Append(callSourceExp.Parse(parameterDictionary, exp.Object));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
