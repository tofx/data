using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryGetMemberNameExpressionNode : SqlDbQueryExpressionNode, IDbQueryExpressionMemberNameParser
    {
        public override string Parse(IDictionary<string, object> parameterDictionary, Expression expressionNode)
        {
            if (expressionNode is MemberExpression)
            {
                var name = ((MemberExpression)expressionNode).Member.Name;

                if (ModelStrategy == null) return name;
                var propQuery = ModelStrategy.PropertyStrategies.Where(c => c.GetPropertyInfo().Name == name).ToList();

                return (propQuery.Any()) ? propQuery.First().GetParameterName() : name;
            }

            if (expressionNode is UnaryExpression)
            {
                var exp = (UnaryExpression)expressionNode;

                if (exp.Operand is MemberExpression)
                {
                    var me = (MemberExpression)exp.Operand;

                    if (me.Expression.NodeType == ExpressionType.Constant)
                    {
                        var constantExp = new SqlDbQueryConstantExpressionNode()
                        {
                            ModelStrategy = ModelStrategy
                        };

                        var value = Expression.Lambda(exp).Compile().DynamicInvoke();
                        return constantExp.Parse(parameterDictionary, Expression.Constant(value));
                    }

                    var name = me.Member.Name;

                    if (ModelStrategy == null) return name;

                    var propQuery = ModelStrategy.PropertyStrategies.Where(c => c.GetPropertyInfo().Name == name).ToList();

                    return propQuery.Any() ? propQuery.First().GetParameterName() : name;
                }

                if (exp.Operand is BinaryExpression)
                {
                    var binaryExp = (BinaryExpression)exp.Operand;
                    var binaryExpStringBuilder = new StringBuilder();

                    // handle left expression.
                    var leftExpNode = SqlDbQueryExpressionFactory.GetNodeParser(binaryExp.Left, ModelStrategy);
                    var rightExpNode = SqlDbQueryExpressionFactory.GetNodeParser(binaryExp.Right, ModelStrategy);

                    binaryExpStringBuilder.Append(leftExpNode.Parse(parameterDictionary, binaryExp.Left));

                    switch (binaryExp.NodeType)
                    {
                        case ExpressionType.Equal:
                            binaryExpStringBuilder.Append(" = ");
                            break;
                        case ExpressionType.NotEqual:
                            binaryExpStringBuilder.Append(" <> ");
                            break;
                        case ExpressionType.GreaterThan:
                            binaryExpStringBuilder.Append(" > ");
                            break;
                        case ExpressionType.GreaterThanOrEqual:
                            binaryExpStringBuilder.Append(" >= ");
                            break;
                        case ExpressionType.LessThan:
                            binaryExpStringBuilder.Append(" < ");
                            break;
                        case ExpressionType.LessThanOrEqual:
                            binaryExpStringBuilder.Append(" <= ");
                            break;
                        case ExpressionType.AndAlso:
                            binaryExpStringBuilder.Append(" AND ");
                            break;
                        case ExpressionType.OrElse:
                            binaryExpStringBuilder.Append(" OR ");
                            break;
                        default:
                            throw new NotSupportedException("ERROR_NOT_SUPPORTED_CONDITION_EXPRESSION_TYPE");
                    }

                    binaryExpStringBuilder.Append(rightExpNode.Parse(parameterDictionary, binaryExp.Right));

                    return binaryExpStringBuilder.ToString();
                }

                if (exp.Operand is MethodCallExpression)
                {
                    var callExpressionNode = new SqlDbQueryCallExpressionNode()
                    {
                        ModelStrategy = ModelStrategy
                    };
                    return callExpressionNode.Parse(parameterDictionary, exp.Operand);
                }

                SqlDbQueryExpressionNode node;

                if (exp.NodeType == ExpressionType.Constant)
                {
                    node = new SqlDbQueryConstantExpressionNode()
                    {
                        ModelStrategy = ModelStrategy
                    };

                    return node.Parse(parameterDictionary, exp);
                }

                if (exp.NodeType != ExpressionType.Convert)
                    throw new NotSupportedException("ERROR_NOT_SUPPORTED_UNARY_EXPRESSION_TYPE");

                node = new SqlDbQueryConstantExpressionNode()
                {
                    ModelStrategy = ModelStrategy
                };

                return node.Parse(parameterDictionary, Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke()));
            }

            throw new NotSupportedException("ERROR_NOT_SUPPORTED_EXPRESSION_TYPE");
        }
    }
}
