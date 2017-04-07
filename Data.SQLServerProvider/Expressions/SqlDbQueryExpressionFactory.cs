using System;
using System.Linq.Expressions;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.Expressions
{
    public class SqlDbQueryExpressionFactory
    {
        public static IDbQueryExpressionNode GetNodeParser(Expression expression, IDbModelStrategy modelStrategy)
        {
            SqlDbQueryExpressionNode expressionNode = null;

            switch (expression.NodeType)
            {
                case ExpressionType.NotEqual:
                    expressionNode = new SqlDbQueryNotEqualExpressionNode();
                    break;

                case ExpressionType.Not:
                    expressionNode = new SqlDbQueryNotExpressionNode();
                    break;

                case ExpressionType.Equal:
                    expressionNode = new SqlDbQueryEqualExpressionNode();
                    break;

                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    expressionNode = new SqlDbQueryGreaterThanExpressionNode();
                    break;

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    expressionNode = new SqlDbQueryLessThanExpressionNode();
                    break;

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    expressionNode = new SqlDbQueryAddExpressionNode();
                    break;

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    expressionNode = new SqlDbQuerySubstractExpressionNode();
                    break;

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    expressionNode = new SqlDbQueryMultiplyExpressionNode();
                    break;

                case ExpressionType.Divide:
                    expressionNode = new SqlDbQueryDivideExpressionNode();
                    break;

                case ExpressionType.MemberAccess:
                    expressionNode = new SqlDbQueryMemberAccessExpressionNode();
                    break;

                case ExpressionType.AndAlso:
                    expressionNode = new SqlDbQueryAndAlsoExpressionNode();
                    break;

                case ExpressionType.OrElse:
                    expressionNode = new SqlDbQueryOrElseExpressionNode();
                    break;

                case ExpressionType.Constant:
                    expressionNode = new SqlDbQueryConstantExpressionNode();
                    break;

                case ExpressionType.Call:
                    expressionNode = new SqlDbQueryCallExpressionNode();
                    break;

                case ExpressionType.Parameter:
                    expressionNode = new SqlDbQueryParameterExpressionNode();
                    break;

                case ExpressionType.Convert:
                    expressionNode = new SqlDbQueryConvertExpressionNode();                    
                    break;
                    
                default:
                    throw new NotSupportedException("ERROR_GIVEN_QUERY_EXPRESSION_TYPE_NOT_SUPPORTED");
            }

            expressionNode.ModelStrategy = modelStrategy;
            return expressionNode;
        }
    }
}
