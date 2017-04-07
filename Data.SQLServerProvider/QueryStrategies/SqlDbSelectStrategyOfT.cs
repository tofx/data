using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using tofx.Data.Abstractions;
using tofx.Data.Providers.SqlServer.Expressions;

namespace tofx.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbSelectStrategy<TModel> : DbQueryStrategyBase<TModel>
        where TModel : class, new()
    {
        private readonly IDbModelStrategy _modelStrategy;
        private string _columns;
        private readonly string _filters, _orderByColumns;
        private readonly long _skipCount, _takeCount;
        private readonly IDictionary<string, object> _parameterDictionary;

        public SqlDbSelectStrategy(
            IDbModelStrategy modelStrategy,
            long skipCount = 0,
            long takeCount = 0,
            IEnumerable<DbQueryWhereClause<TModel>> filterExpressions = null,
            IEnumerable<DbQueryOrderByClause<TModel>> orderByExpressions = null) : base(DbQueryStrategyTypes.Select)
        {
            _modelStrategy = modelStrategy;
            _parameterDictionary = new Dictionary<string, object>();

            if (filterExpressions != null)
                _filters = ParseFilters(filterExpressions);
            if (orderByExpressions != null && orderByExpressions.Any())
                _orderByColumns = ParseOrderByColumns(orderByExpressions);

            _skipCount = skipCount;
            _takeCount = takeCount;
        }

        //public override void DetectQueryRowCount()
        //{
        //    var sqlBuilder = new StringBuilder();
        //    sqlBuilder.Append("SELECT ISNULL(COUNT(*), 0) FROM ");
        //    sqlBuilder.Append(_modelStrategy.DbTableName);
        //    sqlBuilder.Append(" ");

        //    if (!string.IsNullOrEmpty(_filters))
        //    {
        //        sqlBuilder.Append("WHERE ");
        //        sqlBuilder.Append(_filters);
        //    }

        //    var provider = ServiceProvider.Resolve<IDbServiceProvider>();
        //    object count = provider.QueryGetScalar(sqlBuilder.ToString(), null);

        //    if (count != DBNull.Value)
        //        _rowCount = Convert.ToInt32(count);

        //    // calculate paging information.
        //    _pageIdxLast = (_rowCount % PageSize > 0) ? (_rowCount / PageSize) + 1 : _rowCount / PageSize;
        //}

        public override string GetDbQueryScript()
        {
            var sqlBuilder = new StringBuilder();

            if (_skipCount > 0 || _takeCount > 0)
                sqlBuilder.Append("SELECT * FROM (");

            sqlBuilder.Append("SELECT ");

            sqlBuilder.Append(string.IsNullOrEmpty(_columns) ? "*" : _columns);

            if (_skipCount > 0 || _takeCount > 0)
            {
                sqlBuilder.Append(", ROW_NUMBER() OVER (ORDER BY ");

                // add ROW_NUMBER
                if (string.IsNullOrEmpty(_orderByColumns))
                {
                    var keyProperties = _modelStrategy.PropertyStrategies.Where(c => c.IsKey()).ToList();
                    var keyColumnBuilder = new StringBuilder();

                    if (!keyProperties.Any())
                        throw new InvalidOperationException("ModelKeyNotFound");
                    else
                    {
                        foreach (var keyProperty in keyProperties)
                        {
                            if (keyColumnBuilder.Length == 0)
                                keyColumnBuilder.Append(keyProperty.GetParameterName());
                            else
                                keyColumnBuilder.Append(", ").Append(keyProperty.GetParameterName());
                        }

                        sqlBuilder.Append(keyColumnBuilder);
                    }
                }
                else
                {
                    sqlBuilder.Append(_orderByColumns);
                }

                sqlBuilder.Append(")  AS RowPagingIndex");
            }

            sqlBuilder.Append(" ");
            sqlBuilder.Append("FROM [");
            sqlBuilder.Append(_modelStrategy.DbTableName);
            sqlBuilder.Append("] ");

            if (!string.IsNullOrEmpty(_filters))
            {
                sqlBuilder.Append("WHERE ");
                sqlBuilder.Append(_filters);
            }

            if (_skipCount == 0 && _takeCount == 0)
            {
                if (string.IsNullOrEmpty(_orderByColumns)) return sqlBuilder.ToString();

                sqlBuilder.Append("ORDER BY ");
                sqlBuilder.Append(_orderByColumns);

                return sqlBuilder.ToString();
            }

            sqlBuilder.Append(") AS pagingQuery ");

            sqlBuilder.Append(_takeCount == 0
                ? "WHERE pagingQuery.RowPagingIndex > @p__s__index"
                : "WHERE pagingQuery.RowPagingIndex > @p__s__index AND pagingQuery.RowPagingIndex <= @p__e__index");

            return sqlBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> GetDbParameters()
        {
            var parameters = new List<IDbDataParameter>();

            if (!string.IsNullOrEmpty(_filters))
            {
                if (!_parameterDictionary.Any())
                    throw new InvalidOperationException("FilterExistsButParameterNot");

                foreach (var parameter in _parameterDictionary)
                {
                    parameters.Add(new SqlParameter("@" + parameter.Key, parameter.Value));
                }
            }

            if (_skipCount == 0 && _takeCount == 0) return parameters;

            // calculate page row index.
            long startIdx = _skipCount;
            long endIdx = _skipCount + _takeCount;

            parameters.Add(new SqlParameter("@p__s__index", startIdx));
            parameters.Add(new SqlParameter("@p__e__index", endIdx));

            return parameters;
        }

        private string ParseSelectorToColumns(Expression<Func<TModel, object>> selector)
        {
            // check expression type is "New Expression" or "Parameter Expression"
            if (selector.Body is NewExpression)
            {
                var newExpNode = new SqlDbQueryNewExpressionRenderColumnsNode();
                newExpNode.ModelStrategy = _modelStrategy;
                return newExpNode.Parse(_parameterDictionary, selector.Body);
            }

            if (!(selector.Body is ParameterExpression))
                throw new DbEnvironmentException("ERROR_EXPRESSION_NOT_SUPPORTED");

            var paramNode = new SqlDbQueryParameterExpressionRenderColumnsNode();
            paramNode.ModelStrategy = _modelStrategy;
            return paramNode.Parse(_parameterDictionary, selector.Body);
        }

        private string ParseFilters(IEnumerable<DbQueryWhereClause<TModel>> whereExpressions)
        {
            var whereConditionBuilder = new StringBuilder();

            foreach (var whereExpression in whereExpressions)
            {
                if (whereConditionBuilder.Length > 0)
                {
                    switch (whereExpression.Operator)
                    {
                        case DbQueryConditionOperators.And:
                            whereConditionBuilder.Append(" AND ");
                            break;
                        case DbQueryConditionOperators.NotAnd:
                            whereConditionBuilder.Append(" AND NOT ");
                            break;
                        case DbQueryConditionOperators.Or:
                            whereConditionBuilder.Append(" OR ");
                            break;
                        case DbQueryConditionOperators.NotOr:
                            whereConditionBuilder.Append(" OR NOT ");
                            break;
                        case DbQueryConditionOperators.None:
                            throw new InvalidOperationException("FilterConditionMustHaveOperator");
                    }
                }

                whereConditionBuilder.Append("(");
                whereConditionBuilder.Append(ParseExpression(whereExpression.Clause));
                whereConditionBuilder.Append(")");
            }

            return whereConditionBuilder.ToString();
        }

        private string ParseOrderByColumns(IEnumerable<DbQueryOrderByClause<TModel>> orderByExpressions)
        {
            var orderbyBuilder = new StringBuilder();

            foreach (var orderByExpression in orderByExpressions)
            {
                if (orderbyBuilder.Length > 0)
                    orderbyBuilder.Append(", ");

                orderbyBuilder.Append(ParseExpression(orderByExpression.OrderByClause));

                if (orderByExpression.Operator == DbQueryOrderByOperators.Desc)
                    orderbyBuilder.Append(" DESC");
            }

            return orderbyBuilder.ToString();
        }

        private string ParseExpression(Expression<Func<TModel, object>> filterExpressions)
        {
            var parserProvider = ServiceProvider.Resolve<IDbQueryExpressionParserProvider>();

            if (parserProvider == null)
                throw new InvalidOperationException("DbServiceProvider can't find query expression parser provider.");

            var sqlQueryParser = parserProvider.GetExpressionParser();

            switch (filterExpressions.Body.NodeType)
            {
                case ExpressionType.Not:
                    return sqlQueryParser.Parse(_parameterDictionary, (filterExpressions.Body as UnaryExpression), _modelStrategy);

                case ExpressionType.Assign:
                case ExpressionType.AndAlso:
                case ExpressionType.Equal:
                    return sqlQueryParser.Parse(_parameterDictionary, (filterExpressions.Body as BinaryExpression), _modelStrategy);

                case ExpressionType.Convert:
                    return sqlQueryParser.Parse(_parameterDictionary, (filterExpressions.Body as UnaryExpression), _modelStrategy);

                case ExpressionType.MemberAccess:
                    return sqlQueryParser.Parse(_parameterDictionary, (filterExpressions.Body as MemberExpression), _modelStrategy);

                default:
                    throw new NotSupportedException("ERROR_GIVEN_EXPRESSION_NOT_SUPPORTED");
            }
        }
    }
}
