using TOF.Data.Providers.SqlServer.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbSelectAggregateStrategy<TModel> : DbQueryStrategyBase<TModel> where TModel : class, new()
    {
        private DbQueryAggregateMode _aggregMode = DbQueryAggregateMode.Count;
        private IDbModelStrategy _modelStrategy = null;
        private string _columns = null, _filters = null, _groupBySelectors = null;
        private IDictionary<string, object> _parameterDictionary = null;

        public SqlDbSelectAggregateStrategy(
            DbQueryAggregateMode aggregateMode,
            IDbModelStrategy modelStrategy,
            IEnumerable<DbQueryWhereClause<TModel>> filterExpressions = null,
            DbQueryColumnClause<TModel> columnExpression = null,
            IEnumerable<DbQueryGroupByClause<TModel>> groupBySelector = null) : base(DbQueryStrategyTypes.Select)
        {
            _modelStrategy = modelStrategy;
            _aggregMode = aggregateMode;
            _parameterDictionary = new Dictionary<string, object>();

            if (filterExpressions != null)
                _filters = ParseFilters(filterExpressions);
            if (groupBySelector != null)
                _groupBySelectors = ParseGroupByColumns(groupBySelector);

            if (aggregateMode != DbQueryAggregateMode.Any &&
                aggregateMode != DbQueryAggregateMode.Count &&
                aggregateMode != DbQueryAggregateMode.CountBig)
            {
                if (_columns == null)
                    _columns = ParseSelectorToColumn(columnExpression);
            }
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
            sqlBuilder.Append(GenerateSelectBodyStmt());

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

            sqlBuilder.Append(GenerateSelectBodyStmt());

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

            sqlBuilder.Append(GenerateSelectBodyStmt());

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

            sqlBuilder.Append(GenerateSelectBodyStmt());

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

            sqlBuilder.Append(GenerateSelectBodyStmt());

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

            sqlBuilder.Append(GenerateSelectBodyStmt());

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

            sqlBuilder.Append(GenerateSelectBodyStmt());

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

            sqlBuilder.Append(GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
        }

        private string ParseSelectorToColumn(DbQueryColumnClause<TModel> expression)
        {
            var exp = expression.ColumExpression;

            // check expression type is "New Expression" or "Parameter Expression"
            if (exp.Body is NewExpression)
            {
                var newExpNode = new SqlDbQueryNewExpressionRenderColumnsNode();
                newExpNode.ModelStrategy = _modelStrategy;
                return newExpNode.Parse(_parameterDictionary, exp.Body);
            }

            if (exp.Body is UnaryExpression)
            {
                var paramNode = SqlDbQueryExpressionFactory.GetNodeParser(exp.Body, _modelStrategy);
                return paramNode.Parse(_parameterDictionary, exp.Body);
            }

            if (exp.Body is ParameterExpression)
            {
                var paramNode = new SqlDbQueryParameterExpressionRenderColumnsNode();
                paramNode.ModelStrategy = _modelStrategy;
                return paramNode.Parse(_parameterDictionary, exp.Body);
            }

            throw new NotSupportedException("Not supported expression type.");
        }

        private string ParseGroupByColumns(IEnumerable<DbQueryGroupByClause<TModel>> groupBySelectors)
        {
            if (groupBySelectors == null || !groupBySelectors.Any())
                return string.Empty;

            var groupByColumnsBuilder = new StringBuilder();

            foreach (var groupBySelector in groupBySelectors)
            {
                var groupByClause = groupBySelector.Clause;

                // check expression type is "New Expression" or "Parameter Expression"
                if (groupByClause.Body is NewExpression)
                    throw new NotSupportedException("ERROR_GROUP_BY_NOT_SUPPORT_NEW_EXPRESSION");
                else if (groupByClause.Body is UnaryExpression)
                {
                    var paramNode = SqlDbQueryExpressionFactory.GetNodeParser(groupByClause.Body, _modelStrategy);

                    var name = paramNode.Parse(_parameterDictionary, groupByClause.Body);

                    if (groupByColumnsBuilder.Length > 0)
                        groupByColumnsBuilder.Append(", " + name);
                    else
                        groupByColumnsBuilder.Append(name);
                }
                else if (groupByClause.Body is ParameterExpression)
                {
                    var paramNode = new SqlDbQueryParameterExpressionRenderColumnsNode();
                    paramNode.ModelStrategy = _modelStrategy;
                    var name = paramNode.Parse(_parameterDictionary, groupByClause.Body);

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

        private string GenerateSelectBodyStmt()
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

                default:
                    throw new NotSupportedException("ERROR_GIVEN_EXPRESSION_NOT_SUPPORTED");
            }
        }

        public override IEnumerable<IDbDataParameter> GetDbParameters()
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();

            if (!string.IsNullOrEmpty(_filters))
            {
                if (!_parameterDictionary.Any())
                    throw new InvalidOperationException("FilterExistsButParameterNot");

                foreach (var parameter in _parameterDictionary)
                {
                    parameters.Add(new SqlParameter("@" + parameter.Key, parameter.Value));
                }
            }

            return parameters;
        }
    }
}
