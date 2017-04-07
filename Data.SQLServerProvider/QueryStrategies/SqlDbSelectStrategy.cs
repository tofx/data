using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.Expressions;

namespace TOF.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbSelectStrategy : DbQueryStrategyBase
    {
        private IDbModelStrategy _modelStrategy = null;
        private string _columns = null, _filters = null, _orderByColumns = null;
        private bool _enablePaging = false;
        private int _pageIdx = 0, _pageIdxLast = 0, _rowCount = 0;
        private IDictionary<string, object> _parameterDictionary = null;

        public SqlDbSelectStrategy(
            Type modelType,
            IDbModelStrategy modelStrategy,
            int pageSize = 0) : base(modelType)
        {
            _modelStrategy = modelStrategy;
            _parameterDictionary = new Dictionary<string, object>();

            _enablePaging = pageSize > 0;
            _pageIdx = 0;
            _pageIdxLast = 0;
            _rowCount = 0;
        }

        public SqlDbSelectStrategy PrepareColumnSelector<TModel>(Expression<Func<TModel, object>> selector) where TModel: class, new()
        {
            _columns = ParseSelectorToColumns(selector);
            return this;
        }

        public SqlDbSelectStrategy PrepareFilters<TModel>(IEnumerable<DbQueryWhereClause<TModel>> whereExpressions) where TModel : class, new()
        {
            _filters = ParseFilters(whereExpressions);
            return this;
        }

        public SqlDbSelectStrategy PrepareOrderByColumns<TModel>(IEnumerable<DbQueryOrderByClause<TModel>> pagingOrderByExpressions) where TModel : class, new()
        {
            _orderByColumns = ParseOrderByColumns(pagingOrderByExpressions);
            return this;
        }

        public override void DetectQueryRowCount()
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT ISNULL(COUNT(*), 0) FROM [");
            sqlBuilder.Append(_modelStrategy.DbTableName);
            sqlBuilder.Append("] ");

            if (!string.IsNullOrEmpty(_filters))
            {
                sqlBuilder.Append("WHERE ");
                sqlBuilder.Append(_filters);
            }

            var provider = ServiceProvider.Resolve<IDbServiceProvider>();
            object count = provider.QueryGetScalar(sqlBuilder.ToString(), null);

            if (count != DBNull.Value)
                _rowCount = Convert.ToInt32(count);

            // calculate paging information.
            _pageIdxLast = (_rowCount % PageSize > 0) ? (_rowCount / PageSize) + 1 : _rowCount / PageSize;
        }

        public override string GetDbQueryScript()
        {
            var sqlBuilder = new StringBuilder();

            if (_enablePaging)
                sqlBuilder.Append("SELECT * FROM (");

            sqlBuilder.Append("SELECT ");

            if (string.IsNullOrEmpty(_columns))
                sqlBuilder.Append("*");
            else
                sqlBuilder.Append(_columns);

            if (_enablePaging)
            {
                sqlBuilder.Append(", ROW_NUMBER() OVER (ORDER BY ");

                // add ROW_NUMBER
                if (string.IsNullOrEmpty(_orderByColumns))
                    throw new InvalidOperationException("PagingRequiresOrderByClauses");
                else
                    sqlBuilder.Append(_orderByColumns);

                sqlBuilder.Append(")  AS RowPagingIndex");
            }

            sqlBuilder.Append(" ");
            sqlBuilder.Append("FROM ");
            sqlBuilder.Append(_modelStrategy.DbTableName);
            sqlBuilder.Append(" ");

            if (!string.IsNullOrEmpty(_filters))
            {
                sqlBuilder.Append("WHERE ");
                sqlBuilder.Append(_filters);
            }

            if (!_enablePaging)
            {
                if (!string.IsNullOrEmpty(_orderByColumns))
                {
                    sqlBuilder.Append("ORDER BY ");
                    sqlBuilder.Append(_orderByColumns);
                }
            }

            if (_enablePaging)
            {
                sqlBuilder.Append(") AS pagingQuery ");
                sqlBuilder.Append("WHERE pagingQuery.RowPagingIndex >= @p__s__index AND pagingQuery.RowPagingIndex <= @p__e__index");
            }

            return sqlBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> GetDbParameters()
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();

            if (_filters != null)
                parameters = new List<IDbDataParameter>(base.GetDbParameters());

            if (_enablePaging)
            {
                // calculate page row index.
                int idxStart = 0, idxEnd = 0;

                if (_pageIdx == 0)
                {
                    idxStart = 0;
                    idxEnd = PageSize - 1;
                }
                else
                {
                    idxStart = PageSize;
                    idxEnd = (PageSize * _pageIdx) - 1;
                }

                parameters.Add(new SqlParameter("@p__s__index", idxStart));
                parameters.Add(new SqlParameter("@p__e__index", idxEnd));
            }

            return parameters;
        }

        public override void MoveFirst()
        {
            _pageIdx = 0;
        }

        public override void MoveNext()
        {
            _pageIdx++;

            if (_pageIdx > _pageIdxLast)
                _pageIdx = _pageIdxLast;
        }

        public override void MoveLast()
        {
            _pageIdx = _pageIdxLast;
        }

        public override void MovePrevious()
        {
            _pageIdx--;

            if (_pageIdx < 0)
                _pageIdx = 0;
        }

        private string ParseSelectorToColumns<TModel>(Expression<Func<TModel, object>> selector)
        {
            // check expression type is "New Expression" or "Parameter Expression"
            if (selector.Body is NewExpression)
            {
                var newExpNode = new SqlDbQueryNewExpressionRenderColumnsNode();
                newExpNode.ModelStrategy = _modelStrategy;
                return newExpNode.Parse(_parameterDictionary, selector.Body);
            }
            else if (selector.Body is ParameterExpression)
            {
                var paramNode = new SqlDbQueryParameterExpressionRenderColumnsNode();
                paramNode.ModelStrategy = _modelStrategy;
                return paramNode.Parse(_parameterDictionary, selector.Body);
            }
            else
                throw new DbEnvironmentException("ERROR_EXPRESSION_NOT_SUPPORTED");
        }

        private string ParseFilters<TModel>(IEnumerable<DbQueryWhereClause<TModel>> whereExpressions) where TModel : class, new()
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

        private string ParseOrderByColumns<TModel>(IEnumerable<DbQueryOrderByClause<TModel>> pagingOrderByExpressions) where TModel : class, new()
        {
            var orderbyBuilder = new StringBuilder();

            foreach (var orderByExpression in pagingOrderByExpressions)
            {
                if (orderbyBuilder.Length > 0)
                    orderbyBuilder.Append(", ");

                orderbyBuilder.Append(ParseExpression(orderByExpression.OrderByClause));

                if (orderByExpression.Operator == DbQueryOrderByOperators.Desc)
                    orderbyBuilder.Append(" DESC");
            }

            return orderbyBuilder.ToString();
        }

        private string ParseExpression<TModel>(Expression<Func<TModel, object>> filterExpressions)
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

                default:
                    throw new NotSupportedException("ERROR_GIVEN_EXPRESSION_NOT_SUPPORTED");
            }
        }
    }
}
