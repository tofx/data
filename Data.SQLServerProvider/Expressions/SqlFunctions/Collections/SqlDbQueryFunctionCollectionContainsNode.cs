using TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.String;
using System;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.Collections
{
    using System.Collections.Generic;
    using SqlDbQueryFunctionStringContainsNode = SqlDbQueryFunctionStringContainsNode;

    public class SqlDbQueryFunctionCollectionContainsNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Contains", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;

            if (exp.Object != null && exp.Object.Type == typeof(string))
            {
                var expStrParser = new SqlDbQueryFunctionStringContainsNode()
                {
                    ModelStrategy = ModelStrategy
                };
                var expStr = expStrParser.Parse(parameterDictionary, functionExpressionNode);
                expressionBuilder.Append(expStr);
            }
            else
            {
                var collectionValueParser = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments[0], ModelStrategy);
                var fieldParser = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments[1], ModelStrategy);
                var field = fieldParser.Parse(parameterDictionary, exp.Arguments[1]);
                var collectionValue = collectionValueParser.Parse(parameterDictionary, exp.Arguments[0]);
                
                expressionBuilder.Append(field);
                expressionBuilder.Append(" IN ");
                expressionBuilder.Append(collectionValue);
                expressionBuilder.Append(" ");
            }

            return expressionBuilder.ToString();
        }
    }
}
