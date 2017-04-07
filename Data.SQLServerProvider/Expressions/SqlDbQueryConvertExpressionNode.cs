using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryConvertExpressionNode : SqlDbQueryExpressionNode
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var expressionBuilder = new StringBuilder();
            IDbQueryExpressionNode parser = null;

            if (expressionNode is UnaryExpression)
            {
                var unaryExp = expressionNode as UnaryExpression;
                parser = SqlDbQueryExpressionFactory.GetNodeParser(unaryExp.Operand, ModelStrategy);
                expressionBuilder.Append(parser.Parse(parameterDictionary, unaryExp.Operand));
            }
            else
                throw new NotSupportedException();

            return expressionBuilder.ToString();
        }
    }
}
