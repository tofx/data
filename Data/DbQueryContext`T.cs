using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using tofx.Core.DependencyInjection;
using tofx.Core.Infrastructure;
using tofx.Core.Utils;
using tofx.Data.Abstractions;

namespace tofx.Data
{
    public class DbQueryContext<TModel> : IDbQueryContext<TModel> where TModel : class, new()
    {
        private enum QueryItemPosition
        {
            Single,
            First,
            Last,
            ElementAt
        }

        private readonly IDbTable<TModel> _queryTable;
        private readonly IDbServiceProvider _serviceProvider;
        private readonly IDbQueryStrategyProvider _queryStrategyProvider;
        private List<DbQueryGroupByClause<TModel>> _groupByFields;
        private readonly List<DbQueryWhereClause<TModel>> _whereClauses;
        private readonly List<DbQueryOrderByClause<TModel>> _orderbyClauses;
        private readonly Container _container;
        private int _pageSize = 0; // do not paging.
        private long _skipCount = 0, _takeCount = 0;

        public DbQueryContext()
        {
            _container = App.ServiceProviders;
            _groupByFields = new List<DbQueryGroupByClause<TModel>>();
            _whereClauses = new List<DbQueryWhereClause<TModel>>();
            _orderbyClauses = new List<DbQueryOrderByClause<TModel>>();
            _queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
        }

        public DbQueryContext(IDbTable<TModel> dbTable) : this()
        {
            string connectionString = App.Configuration["connectionStrings/default"];
            _queryTable = dbTable;
            _serviceProvider = _container.Resolve<IDbServiceProvider>(connectionString);
        }

        public DbQueryContext(IDbTable<TModel> dbTable, string connectionString) : this()
        {
            _queryTable = dbTable;
            _serviceProvider = _container.Resolve<IDbServiceProvider>(connectionString);
        }

        public DbQueryContext(IDbTable<TModel> dbTable, IDbServiceProvider runtimeProvider) : this()
        {
            _queryTable = dbTable;
            _serviceProvider = runtimeProvider;
        }

        public IDbTable<TModel> Table { get { return _queryTable; } }

        public IDbQueryContext<TModel> Join<TJoinModel>(IDbTable<TJoinModel> table,
            Expression<Func<TModel, object>> innerspecifierExpression,
            Expression<Func<TModel, object>> outerspecifierExpression) where TJoinModel : class, new()
        {
            throw new NotImplementedException();
        }

        public IDbQueryContext<TModel> GroupBy(Expression<Func<TModel, object>> conditionExpression)
        {
            DbQueryGroupByClause<TModel> groupbyClause = new DbQueryGroupByClause<TModel>();
            groupbyClause.Clause = conditionExpression;

            _groupByFields.Add(groupbyClause);

            return this;
        }

        public IDbQueryContext<TModel> OrderBy(Expression<Func<TModel, object>> conditionExpression)
        {
            _orderbyClauses.Clear();

            DbQueryOrderByClause<TModel> orderbyClause = new DbQueryOrderByClause<TModel>();
            orderbyClause.OrderByClause = conditionExpression;
            orderbyClause.Operator = DbQueryOrderByOperators.Asc;

            _orderbyClauses.Add(orderbyClause);

            return this;
        }

        public IDbQueryContext<TModel> OrderByDescending(Expression<Func<TModel, object>> conditionExpression)
        {
            _orderbyClauses.Clear();

            DbQueryOrderByClause<TModel> orderbyClause = new DbQueryOrderByClause<TModel>();
            orderbyClause.OrderByClause = conditionExpression;
            orderbyClause.Operator = DbQueryOrderByOperators.Desc;

            _orderbyClauses.Add(orderbyClause);

            return this;
        }

        public IDbQueryContext<TModel> ThenBy(Expression<Func<TModel, object>> conditionExpression)
        {
            DbQueryOrderByClause<TModel> orderbyClause = new DbQueryOrderByClause<TModel>();
            orderbyClause.OrderByClause = conditionExpression;
            orderbyClause.Operator = DbQueryOrderByOperators.Asc;

            _orderbyClauses.Add(orderbyClause);

            return this;
        }

        public IDbQueryContext<TModel> ThenByDescending(Expression<Func<TModel, object>> conditionExpression)
        {
            DbQueryOrderByClause<TModel> orderbyClause = new DbQueryOrderByClause<TModel>();
            orderbyClause.OrderByClause = conditionExpression;
            orderbyClause.Operator = DbQueryOrderByOperators.Desc;

            _orderbyClauses.Add(orderbyClause);

            return this;
        }

        public IDbQueryContext<TModel> Where(Expression<Func<TModel, object>> conditionExpression, DbQueryConditionOperators Operator = DbQueryConditionOperators.And)
        {
            var clause = new DbQueryWhereClause<TModel>()
            {
                Clause = conditionExpression,
                Operator = Operator
            };

            _whereClauses.Add(clause);

            return this;
        }

        public IDbQueryContext<TModel> Skip(int skipCount)
        {
            _skipCount = skipCount;
            return this;
        }

        public IDbQueryContext<TModel> SkipWhile(int skipCount, Expression<Func<TModel, object>> conditionExpression = null)
        {
            if (conditionExpression == null)
                return Skip(skipCount);

            var clause = new DbQueryWhereClause<TModel>()
            {
                Clause = conditionExpression,
                Operator = DbQueryConditionOperators.And
            };

            _whereClauses.Add(clause);
            _skipCount = skipCount;

            return this;
        }

        public IDbQueryContext<TModel> Take(int itemCount)
        {
            _takeCount = itemCount;
            return this;
        }

        public IDbQueryContext<TModel> TakeWhile(int itemCount, Expression<Func<TModel, object>> conditionExpression = null)
        {
            if (conditionExpression == null)
                return Take(itemCount);

            var clause = new DbQueryWhereClause<TModel>()
            {
                Clause = conditionExpression,
                Operator = DbQueryConditionOperators.And
            };

            _whereClauses.Add(clause);
            _takeCount = itemCount;

            return this;
        }

        public TModel Single(Expression<Func<TModel, object>> conditionExpression = null)
        {
            if (conditionExpression == null)
                return GetDataModelByPosition(QueryItemPosition.Single);

            var clause = new DbQueryWhereClause<TModel>()
            {
                Clause = conditionExpression
            };

            _whereClauses.Add(clause);

            return GetDataModelByPosition(QueryItemPosition.Single);
        }

        public TModel First(Expression<Func<TModel, object>> conditionExpression = null)
        {
            if (conditionExpression == null)
                return GetDataModelByPosition(QueryItemPosition.First);

            var clause = new DbQueryWhereClause<TModel>()
            {
                Clause = conditionExpression
            };

            _whereClauses.Add(clause);

            return GetDataModelByPosition(QueryItemPosition.First);
        }

        public TModel Last(Expression<Func<TModel, object>> conditionExpression = null)
        {
            if (conditionExpression == null)
                return GetDataModelByPosition(QueryItemPosition.Last);

            var clause = new DbQueryWhereClause<TModel>()
            {
                Clause = conditionExpression
            };

            _whereClauses.Add(clause);

            return GetDataModelByPosition(QueryItemPosition.Last);
        }

        public TModel ElementAt(long position, Expression<Func<TModel, object>> conditionExpression = null)
        {
            if (conditionExpression == null)
                return GetDataModelByPosition(QueryItemPosition.ElementAt, position);

            var clause = new DbQueryWhereClause<TModel>()
            {
                Clause = conditionExpression
            };

            _whereClauses.Add(clause);

            return GetDataModelByPosition(QueryItemPosition.ElementAt, position);
        }

        public bool Any(Expression<Func<TModel, object>> specifierExpression)
        {
            var filterExpressions = new List<DbQueryWhereClause<TModel>>()
            {
                new DbQueryWhereClause<TModel>()
                {
                    Clause = specifierExpression,
                    Operator = DbQueryConditionOperators.And
                }
            };

            var strategy = _queryStrategyProvider.GetSelectAggregationStrategy(
                _queryTable.ModelStrategy,
                DbQueryAggregateMode.Any,
                filterExpressions);
            var sql = strategy.GetDbQueryScript();
            var dbparams = strategy.GetDbParameters().ToList();

            _serviceProvider.Open();

            IDataReader reader = null;
            bool isAny;

            try
            {
                reader = _serviceProvider.QueryGetReader(sql, dbparams);
                isAny = reader.Read();
            }
            catch (Exception exception)
            {
                throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", exception, sql, dbparams);
            }
            finally
            {
                reader?.Close();

                _serviceProvider.Close();
            }

            return isAny;
        }

        public int Count(Expression<Func<TModel, object>> specifierExpression, int defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Count, null, specifierExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToInt32(value);
        }

        public long LongCount(Expression<Func<TModel, object>> conditionExpression, long defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.CountBig, null, conditionExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToInt32(value);
        }

        public decimal Average(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Average, columnSpecifier, conditionExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        public decimal Max(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Max, columnSpecifier, conditionExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        public decimal Min(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Min, columnSpecifier, conditionExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        public decimal Sum(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Sum, columnSpecifier, conditionExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            var queryStrategy = _queryStrategyProvider.GetSelectStrategy(
                _queryTable.ModelStrategy, _skipCount, _takeCount, _whereClauses, _orderbyClauses);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters();

            IDataReader reader = null;
            var items = new List<TModel>();

            _serviceProvider.Open();

            try
            {
                reader = _serviceProvider.QueryGetReader(sql, dbparams);

                if (_skipCount == 0 && _takeCount == 0)
                {
                    var records = DbModelBindingHelper.GetDataRecords(reader);

                    items.AddRange(
                        records.Select(record =>
                        DbModelBindingHelper.BindingRecordToModel<TModel>(
                            record, _queryTable.ModelStrategy.PropertyStrategies))
                        );
                }
                else
                {

                    while (reader.Read())
                    {
                        items.Add(DbModelBindingHelper.BindingRecordToModel<TModel>(
                            reader,
                            _queryTable.ModelStrategy.PropertyStrategies));
                    }
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", exception, sql, dbparams);
            }
            finally
            {
                reader?.Close();

                _serviceProvider.Close();
            }

            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private TModel GetDataModelByPosition(QueryItemPosition position, long dataIndex = 0)
        {
            var queryStrategy = _queryStrategyProvider.GetSelectStrategy(
                _queryTable.ModelStrategy, _skipCount, _takeCount, _whereClauses, _orderbyClauses);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters().ToList();

            IDataReader reader = null;
            TModel model = null;
            var singleError = false;

            _serviceProvider.Open();

            try
            {
                reader = _serviceProvider.QueryGetReader(sql, dbparams);
                var dataAvailable = false;
                IDataRecord record = null;
                var idx = 0;

                switch (position)
                {
                    case QueryItemPosition.Single:

                        while (reader.Read())
                        {
                            if (dataAvailable)
                                break;

                            if (idx > 1)
                            {
                                singleError = true;
                                break;
                            }

                            idx++;

                            dataAvailable = true;

                            model = DbModelBindingHelper.BindingRecordToModel<TModel>(
                                reader,
                                _queryTable.ModelStrategy.PropertyStrategies);
                        }

                        break;

                    case QueryItemPosition.ElementAt:

                        idx = -1;

                        while (reader.Read())
                        {
                            idx++;

                            if (idx == dataIndex)
                            {
                                record = reader;
                                model = DbModelBindingHelper.BindingRecordToModel<TModel>(
                                    record, _queryTable.ModelStrategy.PropertyStrategies);
                                break;
                            }
                        }

                        break;

                    case QueryItemPosition.First:

                        reader.Read();
                        record = reader;

                        model = DbModelBindingHelper.BindingRecordToModel<TModel>(
                            record, _queryTable.ModelStrategy.PropertyStrategies);

                        break;

                    case QueryItemPosition.Last:

                        while (reader.Read())
                        {
                            record = reader;
                            model = DbModelBindingHelper.BindingRecordToModel<TModel>(
                                record, _queryTable.ModelStrategy.PropertyStrategies);
                        }

                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", exception, sql, dbparams);
            }
            finally
            {
                reader?.Close();

                _serviceProvider.Close();
            }

            if (singleError)
                throw new InvalidOperationException("ERROR_OUT_OF_SINGLE_RECORD");

            return model;
        }

        private object GetDbQueryAggregationValue(
            DbQueryAggregateMode queryMode,
            Expression<Func<TModel, object>> columnSpecifier,
            Expression<Func<TModel, object>> conditionExpression = null)
        {
            if (queryMode != DbQueryAggregateMode.Any &&
                queryMode != DbQueryAggregateMode.Count &&
                queryMode != DbQueryAggregateMode.CountBig)
            {
                ParameterChecker.NotNull(columnSpecifier);
            }

            IDbQueryStrategy strategy;
            var filterExpressions = new List<DbQueryWhereClause<TModel>>();

            if (conditionExpression != null)
            {
                filterExpressions.Add(new DbQueryWhereClause<TModel>()
                {
                    Clause = conditionExpression,
                    Operator = DbQueryConditionOperators.And
                });
            }

            if (columnSpecifier == null)
            {
                strategy = _queryStrategyProvider.GetSelectAggregationStrategy(
                    _queryTable.ModelStrategy,
                    queryMode,
                    (filterExpressions.Any()) ? filterExpressions : _whereClauses
                    );
            }
            else
            {
                var columnExpression = new DbQueryColumnClause<TModel>()
                {
                    ColumExpression = columnSpecifier
                };

                strategy = _queryStrategyProvider.GetSelectAggregationStrategy(
                    _queryTable.ModelStrategy,
                    queryMode,
                    (filterExpressions.Any()) ? filterExpressions : _whereClauses,
                    columnExpression
                    );
            }

            var sql = strategy.GetDbQueryScript();
            var dbparams = strategy.GetDbParameters();

            _serviceProvider.Open();

            object value;

            try
            {
                value = _serviceProvider.QueryGetScalar(sql, dbparams);
            }
            catch (Exception exception)
            {
                throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", exception, sql, dbparams);
            }
            finally
            {
                _serviceProvider.Close();
            }

            return value;
        }
    }
}
