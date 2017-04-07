using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryNotEqualExpressionNode : SqlDbQueryExpressionNode
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var exp = expressionNode as BinaryExpression;
            var expressionBuilder = new StringBuilder();
            var leftExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Left, ModelStrategy);
            var rightExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Right, ModelStrategy);
            bool leftNull = false, rightNull = false;
            
            if (exp.Left is ConstantExpression)
                leftNull = (exp.Left as ConstantExpression).Value == null;
            if (exp.Right is ConstantExpression)
                rightNull = (exp.Right as ConstantExpression).Value == null;
            
            expressionBuilder.Append("("); 
            expressionBuilder.Append(leftExp.Parse(parameterDictionary, exp.Left));

            if (leftNull || rightNull)
                expressionBuilder.Append(" IS NOT ");
            else
                expressionBuilder.Append(" <> ");

            expressionBuilder.Append(rightExp.Parse(parameterDictionary, exp.Right));            
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
