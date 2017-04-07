using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryExpressionParser : IDbQueryExpressionParser
    {
        public string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode, IDbModelStrategy modelStrategy)
        {
            var queryNode = SqlDbQueryExpressionFactory.GetNodeParser(expressionNode, modelStrategy);

            if (queryNode == null)
                throw new NotSupportedException("ERROR_NODE_TYPE_NOT_SUPPORTED");

            return queryNode.Parse(parameterDictionary, expressionNode);
        }
    }
}
