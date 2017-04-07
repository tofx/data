using System.Collections.Generic;

namespace TOF.Data.Abstractions
{
    public interface IDbQueryStrategyProvider
    {
        IDbQueryStrategy GetInsertStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new();
        IDbQueryStrategy GetUpdateStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new();
        IDbQueryStrategy GetDeleteStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new();
        IDbQueryStrategy GetDeleteAllStrategy(IDbModelStrategy modelStrategy);
        IDbQueryStrategy GetEmptyStrategy(IDbModelStrategy modelStrategy);
        IDbQueryStrategy GetSelectStrategy<TModel>(IDbModelStrategy modelStrategy, long skipCount = 0, long takeCount = 0, IEnumerable<DbQueryWhereClause<TModel>> filterClauses = null, IEnumerable<DbQueryOrderByClause<TModel>> orderByClauses = null) where TModel : class, new();
        IDbQueryStrategy GetSelectAggregationStrategy<TModel>(IDbModelStrategy modelStrategy, DbQueryAggregateMode mode, IEnumerable<DbQueryWhereClause<TModel>> filterClauses = null, DbQueryColumnClause<TModel> columnSpecifier = null, IEnumerable<DbQueryGroupByClause<TModel>> groupByClauses = null) where TModel : class, new();
        IDbQueryValueIncreDecreStrategy GetValueIncreaseStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new();
        IDbQueryValueIncreDecreStrategy GetValueDecreaseStrategy<TModel>(IDbModelStrategy modelStrategy) where TModel : class, new();
    }
}
