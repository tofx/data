using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryNewExpressionRenderColumnsNode : SqlDbQueryExpressionNode
    {
        private bool _convertPropertyNameToDbName = true;

        public SqlDbQueryNewExpressionRenderColumnsNode(bool convertPropertyNameToDbName = true)
        {
            _convertPropertyNameToDbName = convertPropertyNameToDbName;
        }

        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            var sqlsb = new StringBuilder();
            var newExp = expressionNode as NewExpression;

            if (_convertPropertyNameToDbName)
            {
                foreach (var memberAccessItem in newExp.Arguments)
                {
                    var memberAccessNode = SqlDbQueryExpressionFactory.GetNodeParser(memberAccessItem, ModelStrategy);

                    if (sqlsb.Length == 0)
                        sqlsb.Append(memberAccessNode.Parse(parameterDictionary, memberAccessItem));
                    else
                    {
                        sqlsb.Append(", ");
                        sqlsb.Append(memberAccessNode.Parse(parameterDictionary, memberAccessItem));
                    }
                }
            }
            else
            {
                foreach (var memberAccessItem in newExp.Arguments)
                {
                    SqlDbQueryExpressionNode memberAccessNode = new SqlDbQueryMemberAccessExpressionNode();
                    memberAccessNode.ModelStrategy = ModelStrategy;

                    if (sqlsb.Length == 0)
                        sqlsb.Append(memberAccessNode.Parse(parameterDictionary, memberAccessItem));
                    else
                    {
                        sqlsb.Append(", ");
                        sqlsb.Append(memberAccessNode.Parse(parameterDictionary, memberAccessItem));
                    }
                }
            }

            return sqlsb.ToString();
        }
    }
}
