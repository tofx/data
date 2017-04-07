using tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.Collections;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    using System.Collections.Generic;
    using SqlQueryFunctionCollectionContainsNode = SqlDbQueryFunctionCollectionContainsNode;

    public class SqlDbQueryFunctionStringContainsNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Contains", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;

            if (exp.Object == null)
            {
                var expParser = new SqlQueryFunctionCollectionContainsNode()
                {
                    ModelStrategy = ModelStrategy
                };
                var expStr = expParser.Parse(parameterDictionary, functionExpressionNode);
                expressionBuilder.Append(expStr);
            }
            else
            {
                var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp, ModelStrategy);
                var argExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), ModelStrategy);
                var callSourceExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Object, ModelStrategy);
                var argument = argExp.Parse(parameterDictionary, exp.Arguments.First());

                // handling parameter, remove first and last character.
                argument = argument.Substring(1);
                argument = argument.Substring(0, argument.Length - 1);

                expressionBuilder.Append(callSourceExp.Parse(parameterDictionary, exp.Object));
                expressionBuilder.Append(" LIKE '%");
                expressionBuilder.Append(argument);
                expressionBuilder.Append("%' ");
            }

            return expressionBuilder.ToString();
        }
    }
}
