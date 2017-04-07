using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryConstantExpressionNode : SqlDbQueryExpressionNode
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = expressionNode as ConstantExpression;

            if (exp.Value == null)
                expressionBuilder.Append("NULL");
            else
            {
                // create parameter.
                string pname = ParameterUtils.CreateNewParameter(ref parameterDictionary);
                parameterDictionary[pname] = exp.Value.ToString();
                expressionBuilder.Append("@" + pname);

                //if (exp.Value.GetType() == typeof(string))
                //{
                //    ParameterDictionary[pname] = exp.Value.ToString();
                //    expressionBuilder.Append("@" + pname);
                //}
                //else if (exp.Value.GetType() == typeof(DateTime))
                //{
                //    ParameterDictionary[pname] = Convert.ToDateTime(exp.Value);
                //    expressionBuilder.Append("@" + pname);
                //}
                //else
            }

            return expressionBuilder.ToString();
        }
    }
}
