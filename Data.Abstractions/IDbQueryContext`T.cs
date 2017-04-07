using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TOF.Data.Abstractions
{
    public interface IDbQueryContext<TModel> : IEnumerable<TModel> where TModel: class, new()
    {
        IDbTable<TModel> Table { get; }

        #region Data Query Operators
        IDbQueryContext<TModel> Join<TJoinModel>(
            IDbTable<TJoinModel> table,
            Expression<Func<TModel, object>> innerSpecifierExpression,
            Expression<Func<TModel, object>> outerSpecifierExpression) where TJoinModel : class, new();
        IDbQueryContext<TModel> Where(
            Expression<Func<TModel, object>> specifierExpression,
            DbQueryConditionOperators Operator = DbQueryConditionOperators.And);
        IDbQueryContext<TModel> OrderBy(Expression<Func<TModel, object>> sortExpression);
        IDbQueryContext<TModel> OrderByDescending(Expression<Func<TModel, object>> sortExpression);
        IDbQueryContext<TModel> ThenBy(Expression<Func<TModel, object>> sortExpression);
        IDbQueryContext<TModel> ThenByDescending(Expression<Func<TModel, object>> sortExpression);
        IDbQueryContext<TModel> GroupBy(Expression<Func<TModel, object>> groupByExpression);
        TModel Single(Expression<Func<TModel, object>> conditionExpression = null);
        TModel First(Expression<Func<TModel, object>> conditionExpression = null);
        TModel Last(Expression<Func<TModel, object>> conditionExpression = null);
        TModel ElementAt(long position, Expression<Func<TModel, object>> conditionExpression = null);

        #endregion

        #region Aggregation Operators
        bool Any(Expression<Func<TModel, object>> conditionExpression);
        int Count(Expression<Func<TModel, object>> conditionExpression, int defaultValue = 0);
        long LongCount(Expression<Func<TModel, object>> conditionExpression, long defaultValue = 0);
        decimal Average(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);
        decimal Max(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);
        decimal Min(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);
        decimal Sum(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> conditionExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);

        #endregion

        #region Paging Operators

        IDbQueryContext<TModel> Skip(int skipCount);
        IDbQueryContext<TModel> SkipWhile(int skipCount, Expression<Func<TModel, object>> conditionExpression = null);
        IDbQueryContext<TModel> Take(int itemCount);
        IDbQueryContext<TModel> TakeWhile(int itemCount, Expression<Func<TModel, object>> conditionExpression = null);

        #endregion
    }
}
