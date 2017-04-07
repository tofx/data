using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionConcatNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Concat", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp, ModelStrategy);
            var argExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), ModelStrategy);

            var arguments = exp.Arguments;
            bool firstarg = true;

            expressionBuilder.Append("CONCAT(");

            foreach (var argument in arguments)
            {
                if (!firstarg)
                    expressionBuilder.Append(", ");

                expressionBuilder.Append(argExp.Parse(parameterDictionary, argument));
            }

            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
