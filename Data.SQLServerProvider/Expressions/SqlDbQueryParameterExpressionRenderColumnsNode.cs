using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryParameterExpressionRenderColumnsNode : SqlDbQueryExpressionNode
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var paramExp = expressionNode as ParameterExpression;
            var paramProperties = paramExp.Type.GetProperties();
            var sb = new StringBuilder();

            foreach (var prop in paramProperties)
            {
                if (sb.Length == 0)
                    sb.Append(paramExp.Name + "." + prop.Name);
                else
                {
                    sb.Append(", ");
                    sb.Append(paramExp.Name + "." + prop.Name);
                }
            }

            return sb.ToString();
        }
    }
}
