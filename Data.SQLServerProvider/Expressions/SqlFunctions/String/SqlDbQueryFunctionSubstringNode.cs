using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionSubstringNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("Substring", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp.Object, ModelStrategy);
            var argExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), ModelStrategy);
            int startIndex = 0, length = 0;

            var arguments = exp.Arguments;

            if (arguments.Count() == 1)
                length = Convert.ToInt32(argExp.Parse(parameterDictionary, exp.Arguments.ElementAt(0)));
            else
            {
                startIndex = Convert.ToInt32(argExp.Parse(parameterDictionary, exp.Arguments.ElementAt(0)));
                length = Convert.ToInt32(argExp.Parse(parameterDictionary, exp.Arguments.ElementAt(1)));
            }

            startIndex++; // fix different of T-SQL (start from 1) and CLR (start from 0).

            expressionBuilder.Append("SUBSTRING(");
            expressionBuilder.Append(expParser.Parse(parameterDictionary, exp.Object));
            expressionBuilder.Append(string.Format(", {0}, {1}", startIndex, length));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
