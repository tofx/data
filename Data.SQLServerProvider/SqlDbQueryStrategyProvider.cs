using System;
using System.Collections.Generic;
using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.QueryStrategies;

namespace TOF.Data.Providers.SqlServer
{
    public class SqlDbQueryStrategyProvider : IDbQueryStrategyProvider
    {
        public IDbQueryStrategy GetSelectAggregationStrategy(DbQueryAggregateMode mode, Type modelType, IDbModelStrategy modelStrategy)
        {
            return new SqlDbSelectAggregateStrategy(mode, modelType, modelStrategy);
        }

        public IDbQueryStrategy GetSelectAggregationStrategy<TModel>(IDbModelStrategy modelStrategy, DbQueryAggregateMode mode, IEnumerable<DbQueryWhereClause<TModel>> dataFilterOperators = null, DbQueryColumnClause<TModel> columnSpecifier = null, IEnumerable<DbQueryGroupByClause<TModel>> groupBySelector = null) where TModel : class, new()
        {
            return new SqlDbSelectAggregateStrategy<TModel>(mode, modelStrategy, dataFilterOperators, columnSpecifier, groupBySelector);
        }

        public IDbQueryValueIncreDecreStrategy GetValueDecreaseStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new()
        {
            return new SqlDbUpdateStrategy<TModel>(modelStrategy);
        }

        public IDbQueryValueIncreDecreStrategy GetValueIncreaseStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new()
        {
            return new SqlDbUpdateStrategy<TModel>(modelStrategy);
        }

        public IDbQueryStrategy GetDeleteStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbDeleteStrategy(modelStrategy);
        }

        public IDbQueryStrategy GetDeleteStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new()
        {
            return new SqlDbDeleteStrategy<TModel>(modelStrategy);
        }

        public IDbQueryStrategy GetInsertStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbInsertStrategy(modelStrategy);
        }

        public IDbQueryStrategy GetInsertStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new()
        {
            return new SqlDbInsertStrategy<TModel>(modelStrategy);
        }

        public IDbQueryStrategy GetSelectStrategy(Type modelType, IDbModelStrategy modelStrategy)
        {
            return new SqlDbSelectStrategy(modelType, modelStrategy);
        }

        public IDbQueryStrategy GetSelectStrategy<TModel>(IDbModelStrategy modelStrategy, long skipCount = 0, long takeCount = 0, IEnumerable<DbQueryWhereClause<TModel>> dataFilterOperators = null, IEnumerable<DbQueryOrderByClause<TModel>> dataSortOperators = null) where TModel : class, new()
        {
            return new SqlDbSelectStrategy<TModel>(modelStrategy, skipCount, takeCount, dataFilterOperators, dataSortOperators);
        }

        public IDbQueryStrategy GetUpdateStrategy(Type modelType, IDbModelStrategy modelStrategy)
        {
            return new SqlDbUpdateStrategy(modelStrategy);
        }

        public IDbQueryStrategy GetUpdateStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new()
        {
            return new SqlDbUpdateStrategy<TModel>(modelStrategy);
        }

        public IDbQueryStrategy GetDeleteAllStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbDeleteAllStrategy(modelStrategy);
        }

        public IDbQueryStrategy GetEmptyStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbEmptyStrategy(modelStrategy);
        }
    }
}
