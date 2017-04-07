using tofx.Core.DependencyInjection;
using tofx.Core.Infrastructure;
using tofx.Data.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace tofx.Data
{
    public class DbTable<TModel> : IDbTable<TModel> where TModel : class, new()
    {
        private readonly IDbServiceProvider _serviceProvider;
        private readonly Container _container;

        public DbTable()
        {
            _container = App.ServiceProviders;
        }

        public DbTable(IDbModelStrategy modelStrategy, IDbServiceProvider dbServiceProvider) : this()
        {
            ModelStrategy = modelStrategy;
            _serviceProvider = dbServiceProvider;
        }

        public IDbModelStrategy ModelStrategy { get; }
        public bool IsView => false;
        public string Name => ModelStrategy.DbTableName;
        public DbObjectTypes Type => DbObjectTypes.Table;

        public void Delete(TModel model)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetDeleteStrategy<TModel>(ModelStrategy);

            ExecuteModelDbOperation(strategy, model);
        }

        public void Delete(IEnumerable<TModel> models)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetDeleteStrategy<TModel>(ModelStrategy);

            ExecuteModelDbOperation(strategy, models);
        }

        public void Insert(TModel model)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetInsertStrategy<TModel>(ModelStrategy);

            ExecuteModelDbOperation(strategy, model);
        }

        public void Insert(IEnumerable<TModel> models)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetInsertStrategy<TModel>(ModelStrategy);

            ExecuteModelDbOperation(strategy, models);
        }

        public void Update(TModel model)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetUpdateStrategy<TModel>(ModelStrategy);

            ExecuteModelDbOperation(strategy, model);
        }

        public void Update(IEnumerable<TModel> models)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetUpdateStrategy<TModel>(ModelStrategy);

            ExecuteModelDbOperation(strategy, models);
        }

        public IDbQueryContext<TModel> Join<TJoinModel>(
            IDbTable<TJoinModel> table,
            Expression<Func<TModel, object>> innerSpecifierExpression,
            Expression<Func<TModel, object>> outerSpecifierExpression) where TJoinModel : class, new()
        {
            var queryContext = new DbQueryContext<TModel>(this, _serviceProvider);
            return queryContext.Join(table, innerSpecifierExpression, outerSpecifierExpression);
        }

        public IDbQueryContext<TModel> Where(Expression<Func<TModel, object>> specifierExpression, DbQueryConditionOperators Operator = DbQueryConditionOperators.And)
        {
            var queryContext = new DbQueryContext<TModel>(this, _serviceProvider).Where(specifierExpression, Operator);
            return queryContext;
        }

        public IDbQueryContext<TModel> OrderBy(Expression<Func<TModel, object>> specifierExpression)
        {
            var queryContext = new DbQueryContext<TModel>(this, _serviceProvider).OrderBy(specifierExpression);
            return queryContext;
        }

        public IDbQueryContext<TModel> OrderByDescending(Expression<Func<TModel, object>> specifierExpression)
        {
            var queryContext = new DbQueryContext<TModel>(this, _serviceProvider).OrderByDescending(specifierExpression);
            return queryContext;
        }

        public void Empty()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetEmptyStrategy(ModelStrategy);

            ExecuteModelDbOperation(strategy);
        }

        public void DeleteAll()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetDeleteAllStrategy(ModelStrategy);

            ExecuteModelDbOperation(strategy);
        }

        public bool Any(Expression<Func<TModel, object>> specifierExpression)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var filterExpressions = new List<DbQueryWhereClause<TModel>>()
            {
                new DbQueryWhereClause<TModel>()
                {
                    Clause = specifierExpression,
                    Operator = DbQueryConditionOperators.And
                }
            };

            var strategy = queryStrategyProvider.GetSelectAggregationStrategy(
                ModelStrategy,
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

        public decimal Average(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Average, columnSpecifier, specifierExpression, groupByExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        public int Count(Expression<Func<TModel, object>> specifierExpression, int defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Count, specifierExpression: specifierExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToInt32(value);
        }

        public long CountLong(Expression<Func<TModel, object>> specifierExpression, long defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.CountBig, specifierExpression: specifierExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToInt32(value);
        }

        public decimal Max(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Max, columnSpecifier, specifierExpression, groupByExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        public decimal Min(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Min, columnSpecifier, specifierExpression, groupByExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        public decimal Sum(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0)
        {
            var value = GetDbQueryAggregationValue(
                DbQueryAggregateMode.Sum, columnSpecifier, specifierExpression, groupByExpression);

            return (value == null || value == DBNull.Value)
                ? defaultValue
                : Convert.ToDecimal(value);
        }

        private object GetDbQueryAggregationValue(DbQueryAggregateMode queryMode, Expression<Func<TModel, object>> columnSpecifier = null, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null)
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var filterExpressions = new List<DbQueryWhereClause<TModel>>();
            var groupByExpressions = new List<DbQueryGroupByClause<TModel>>();

            if (specifierExpression != null)
            {
                filterExpressions.Add(new DbQueryWhereClause<TModel>()
                {
                    Clause = specifierExpression,
                    Operator = DbQueryConditionOperators.And
                });
            }

            if (groupByExpression != null)
            {
                groupByExpressions.Add(new DbQueryGroupByClause<TModel>()
                {
                    Clause = groupByExpression
                });
            }

            var strategy = queryStrategyProvider.GetSelectAggregationStrategy(
                ModelStrategy,
                queryMode,
                filterExpressions,
                new DbQueryColumnClause<TModel>() { ColumExpression = columnSpecifier },
                groupByExpressions);
            var sql = strategy.GetDbQueryScript();
            var dbparams = strategy.GetDbParameters().ToList();

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

        public IEnumerator<TModel> GetEnumerator()
        {
            return (new DbQueryContext<TModel>(this, _serviceProvider)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ExecuteModelDbOperation(IDbQueryStrategy strategy, TModel model = null)
        {
            Exception exception = null;
            var dbparams = (model == null)
                ? new List<IDbDataParameter>()
                : DbModelBindingHelper.BindingModelToParameters(model, ModelStrategy.PropertyStrategies, strategy.GetDbParameters());

            _serviceProvider.Open();

            try
            {
                if (_serviceProvider.Execute(strategy.GetDbQueryScript(), dbparams) == 0)
                    throw new DbChangeNoAffectException();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            _serviceProvider.Close();

            if (exception != null)
                throw new DbAggregateException(exception);
        }

        private void ExecuteModelDbOperation(IDbQueryStrategy strategy, IEnumerable<TModel> models)
        {
            var exceptions = new Dictionary<int, Exception>();

            _serviceProvider.Open();
            _serviceProvider.BeginTransaction();

            var modelIdx = 0;

            foreach (var model in models)
            {
                try
                {
                    var dbparams = DbModelBindingHelper.BindingModelToParameters(
                        model, ModelStrategy.PropertyStrategies, strategy.GetDbParameters());
                    if (_serviceProvider.Execute(strategy.GetDbQueryScript(), dbparams) == 0)
                        throw new DbChangeNoAffectException();
                }
                catch (Exception ex)
                {
                    exceptions.Add(modelIdx, ex);
                }

                modelIdx++;
            }

            if (!exceptions.Any())
            {
                _serviceProvider.Commit();
                _serviceProvider.Close();
            }
            else
            {
                _serviceProvider.Rollback();
                _serviceProvider.Close();
                throw new DbAggregateException(exceptions);
            }
        }
    }
}
