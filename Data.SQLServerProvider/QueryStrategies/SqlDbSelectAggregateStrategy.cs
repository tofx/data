using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.Expressions;

namespace TOF.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbSelectAggregateStrategy : DbQueryStrategyBase
    {
        private DbQueryAggregateMode _aggregMode = DbQueryAggregateMode.Count;
        private IDbModelStrategy _modelStrategy = null;
        private string _columns = null, _filters = null, _groupBySelectors = null;
        private IDictionary<string, object> _parameterDictionary = null;

        public SqlDbSelectAggregateStrategy(
            DbQueryAggregateMode aggregateMode,
            Type modelType,
            IDbModelStrategy modelStrategy) : base(modelType)
        {
            _modelStrategy = modelStrategy;
            _aggregMode = aggregateMode;
            _parameterDictionary = new Dictionary<string, object>();
        }

        public SqlDbSelectAggregateStrategy PrepareColumnSelector<TModel>(Expression<Func<TModel, object>> selector)
        {
            _columns = ParseSelectorToColumns(selector);
            return this;
        }

        public SqlDbSelectAggregateStrategy PrepareFilters<TModel>(IEnumerable<DbQueryWhereClause<TModel>> filterExpressions) where TModel: class, new()
        {
            _filters = ParseFilters(filterExpressions);
            return this;
        }

        public SqlDbSelectAggregateStrategy PrepareGroupBySelector<TModel>(Expression<Func<TModel, object>>[] groupBySelectors)
        {
            _groupBySelectors = ParseGroupByColumns(groupBySelectors);
            return this;
        }

        public override IEnumerable<IDbDataParameter> GetDbParameters()
        {
            return base.GetDbParameters();
        }

        public override string GetDbQueryScript()
        {
            switch (_aggregMode)
            {
                case DbQueryAggregateMode.Any:
                    return ParseAnyQuery();
                case DbQueryAggregateMode.Count:
                    return ParseCountQuery();
                case DbQueryAggregateMode.CountBig:
                    return ParseCountQuery(true);
                case DbQueryAggregateMode.Average:
                    return ParseAverageQuery();
                case DbQueryAggregateMode.Sum:
                    return ParseSumQuery();
                case DbQueryAggregateMode.Max:
                    return ParseMaxQuery();
                case DbQueryAggregateMode.Min:
                    return ParseMinQuery();
                case DbQueryAggregateMode.Var:
                    return ParseVarQuery();
                case DbQueryAggregateMode.VarP:
                    return ParseVarQuery(true);
                case DbQueryAggregateMode.StdDev:
                    return ParseStdDevQuery();
                case DbQueryAggregateMode.StdDevP:
                    return ParseStdDevQuery(true);
                default:
                    throw new NotSupportedException("ERROR_UNKNOWN_AGGREGATE_MODE");
            }
        }

        private string ParseAnyQuery()
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("SELECT TOP 1 *");
            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
        }

        private string ParseCountQuery(bool isBigCount = false)
        {
            var sqlBuilder = new StringBuilder();
            var aggOp = (isBigCount) ? "COUNT_BIG" : "COUNT";

            if (!string.IsNullOrEmpty(_groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {1}, ISNULL({0}({2}), 0) AS {1}", aggOp, _groupBySelectors, _columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL({0}(*), 0)", aggOp));

            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
        }

        private string ParseAverageQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (_columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_AVERAGE_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(_groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(AVG({1}), 0.0) AS {1}", _groupBySelectors, _columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(AVG({0}), 0.0) As {0}", _columns));

            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
        }

        private string ParseSumQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (_columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_AVERAGE_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(_groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(SUM({1}), 0.0) AS {1}", _groupBySelectors, _columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(SUM({0}), 0.0) AS {0}", _columns));

            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
        }

        private string ParseMaxQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (_columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_MAX_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(_groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(MAX({1}), 0.0) AS {1}", _groupBySelectors, _columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(MAX({0}), 0.0) AS {0}", _columns));

            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
        }

        private string ParseMinQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (_columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_MIN_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(_groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(MIN({1}), 0.0) AS {1}", _groupBySelectors, _columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(MIN({0}), 0.0) AS {0}", _columns));

            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
        }

        private string ParseVarQuery(bool isForPopulation = false)
        {
            var sqlBuilder = new StringBuilder();
            var aggOp = (isForPopulation) ? "VARP" : "VAR";

            if (!string.IsNullOrEmpty(_groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {1}, ISNULL({0}({2}), 0) AS {1}", aggOp, _groupBySelectors, _columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL({0}(*), 0)", aggOp));

            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
        }

        private string ParseStdDevQuery(bool isForPopulation = false)
        {
            var sqlBuilder = new StringBuilder();
            var aggOp = (isForPopulation) ? "STDEVP" : "STDEV";

            if (!string.IsNullOrEmpty(_groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {1}, ISNULL({0}({2}), 0) AS {1}", aggOp, _groupBySelectors, _columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL({0}(*), 0)", aggOp));

            sqlBuilder.Append(GenerateSelectFromStmt());

            return sqlBuilder.ToString();
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
            else if (selector.Body is UnaryExpression)
            {
                var paramNode = SqlDbQueryExpressionFactory.GetNodeParser(selector.Body, _modelStrategy);
                return paramNode.Parse(_parameterDictionary, selector.Body);
            }
            else if (selector.Body is ParameterExpression)
            {
                var paramNode = new SqlDbQueryParameterExpressionRenderColumnsNode();
                paramNode.ModelStrategy = _modelStrategy;
                return paramNode.Parse(_parameterDictionary, selector.Body);
            }
            else
                throw new DbEnvironmentException(
                    "ERROR_EXPRESSION_NOT_SUPPORTED");
        }

        private string ParseGroupByColumns<TModel>(Expression<Func<TModel, object>>[] groupBySelectors)
        {
            if (groupBySelectors == null || groupBySelectors.Length == 0)
                return string.Empty;

            var groupByColumnsBuilder = new StringBuilder();

            foreach (var groupBySelector in groupBySelectors)
            {
                // check expression type is "New Expression" or "Parameter Expression"
                if (groupBySelector.Body is NewExpression)
                    throw new NotSupportedException("ERROR_GROUP_BY_NOT_SUPPORT_NEW_EXPRESSION");
                else if (groupBySelector.Body is UnaryExpression)
                {
                    var paramNode = SqlDbQueryExpressionFactory.GetNodeParser(groupBySelector.Body, _modelStrategy);
                    var name = paramNode.Parse(_parameterDictionary, groupBySelector.Body);

                    if (groupByColumnsBuilder.Length > 0)
                        groupByColumnsBuilder.Append(", " + name);
                    else
                        groupByColumnsBuilder.Append(name);
                }
                else if (groupBySelector.Body is ParameterExpression)
                {
                    var paramNode = new SqlDbQueryParameterExpressionRenderColumnsNode();
                    paramNode.ModelStrategy = _modelStrategy;
                    var name = paramNode.Parse(_parameterDictionary, groupBySelector.Body);

                    if (groupByColumnsBuilder.Length > 0)
                        groupByColumnsBuilder.Append(", " + name);
                    else
                        groupByColumnsBuilder.Append(name);
                }
                else
                    throw new DbEnvironmentException(
                        "ERROR_EXPRESSION_NOT_SUPPORTED");
            }

            return groupByColumnsBuilder.ToString();
        }

        private string ParseFilters<TModel>(IEnumerable<DbQueryWhereClause<TModel>> whereExpressions) where TModel: class, new()
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

        private string GenerateSelectFromStmt()
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(" ");
            sqlBuilder.Append("FROM [");
            sqlBuilder.Append(_modelStrategy.DbTableName);
            sqlBuilder.Append("] ");

            if (!string.IsNullOrEmpty(_filters))
            {
                sqlBuilder.Append("WHERE ");
                sqlBuilder.Append(_filters);
            }

            if (!string.IsNullOrEmpty(_groupBySelectors))
            {
                sqlBuilder.Append("GROUP BY ");
                sqlBuilder.Append(_groupBySelectors);
            }

            return sqlBuilder.ToString();
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
