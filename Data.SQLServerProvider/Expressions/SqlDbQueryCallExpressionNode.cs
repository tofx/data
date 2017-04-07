using tofx.Data.Providers.SqlServer.Expressions.SqlFunctions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryCallExpressionNode : SqlDbQueryExpressionNode
    {
        private static readonly IEnumerable<IDbQueryFunctionNode> CallExpressionNodes;

        static SqlDbQueryCallExpressionNode()
        {
            CallExpressionNodes = SqlDbFunctionsLoader.Load();
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var expressionBuilder = new StringBuilder();            
            var found = false;
            string methodName;

            if (expressionNode is MethodCallExpression)
                methodName = ((MethodCallExpression)expressionNode).Method.Name;
            else
                methodName = ((MemberExpression)expressionNode).Member.Name;

            expressionBuilder.Append("(");

            foreach (var functionNode in CallExpressionNodes)
            {
                if (!functionNode.CheckForHandle(methodName)) continue;

                ((SqlDbQueryExpressionNode)functionNode).ModelStrategy = ModelStrategy;
                expressionBuilder.Append(functionNode.Parse(parameterDictionary, expressionNode));
                found = true;
                break;
            }

            if (!found)
                throw new NotSupportedException();

            expressionBuilder.Append(")");
            
            return expressionBuilder.ToString();
        }
    }
}
