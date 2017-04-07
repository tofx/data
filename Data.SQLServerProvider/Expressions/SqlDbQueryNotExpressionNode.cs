using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryNotExpressionNode : SqlDbQueryExpressionNode
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var exp = expressionNode as UnaryExpression;
            var expressionBuilder = new StringBuilder();
            var opExpressionParser = SqlDbQueryExpressionFactory.GetNodeParser(exp.Operand, ModelStrategy);

            if (exp.Operand.Type == typeof(bool))
            {
                string pname = ParameterUtils.CreateNewParameter(ref parameterDictionary);

                expressionBuilder.Append("(");
                expressionBuilder.Append(opExpressionParser.Parse(parameterDictionary, exp.Operand));
                expressionBuilder.Append(string.Format(" = @{0})", pname));
                parameterDictionary[pname] = 0;
            }
            else
            {
                expressionBuilder.Append("( NOT (");
                expressionBuilder.Append(opExpressionParser.Parse(parameterDictionary, exp.Operand));
                expressionBuilder.Append(") )");
            }

            return expressionBuilder.ToString();
        }
    }
}
