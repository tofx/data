using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryOrElseExpressionNode : SqlDbQueryExpressionNode
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = expressionNode as BinaryExpression;
            var leftExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Left, ModelStrategy);
            var rightExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Right, ModelStrategy);

            expressionBuilder.Append("(");
            expressionBuilder.Append(leftExp.Parse(parameterDictionary, exp.Left));
            expressionBuilder.Append(" OR ");
            expressionBuilder.Append(rightExp.Parse(parameterDictionary, exp.Right));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
