﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions.SqlFunctions.String
{
    public class SqlDbQueryFunctionToLowerNode : SqlDbQueryExpressionNode, IDbQueryFunctionNode
    {   
        public bool CheckForHandle(string functionName)
        {
            return string.Equals("ToLower", functionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression functionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = functionExpressionNode as MethodCallExpression;
            var expParser = SqlDbQueryExpressionFactory.GetNodeParser(exp.Object, ModelStrategy);

            expressionBuilder.Append("LOWER(");
            expressionBuilder.Append(expParser.Parse(parameterDictionary, exp.Object));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
