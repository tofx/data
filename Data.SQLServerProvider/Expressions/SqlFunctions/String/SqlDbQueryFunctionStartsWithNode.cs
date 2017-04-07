using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionStartsWithNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("StartsWith", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;

            if (exp.Object == null)
                throw new NotSupportedException();
            else
            {
                var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp, ModelStrategy);
                var argExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), ModelStrategy);
                var callSourceExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Object, ModelStrategy);
                var argument = argExp.Parse(parameterDictionary, exp.Arguments.First());

                expressionBuilder.Append(callSourceExp.Parse(parameterDictionary, exp.Object));
                expressionBuilder.Append(" LIKE ");
                expressionBuilder.Append(argument.Substring(0, argument.Length - 1));
                expressionBuilder.Append("%' ");
            }

            return expressionBuilder.ToString();
        }
    }
}
