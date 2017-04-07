using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryDivideExpressionNode : SqlDbQueryExpressionNode
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = expressionNode as BinaryExpression;
            var leftExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Left, ModelStrategy);
            var rightExp = SqlDbQueryExpressionFactory.GetNodeParser(exp.Right, ModelStrategy);

            expressionBuilder.Append(leftExp.Parse(parameterDictionary, exp.Left));
            expressionBuilder.Append(" / ");
            expressionBuilder.Append(rightExp.Parse(parameterDictionary, exp.Right));

            return expressionBuilder.ToString();
        }
    }
}
