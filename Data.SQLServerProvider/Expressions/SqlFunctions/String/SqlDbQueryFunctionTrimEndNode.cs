using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionTrimEndNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("TrimEnd", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp.Object, ModelStrategy);

            expressionBuilder.Append("RTRIM(");
            expressionBuilder.Append(expParser.Parse(parameterDictionary, exp.Object));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
