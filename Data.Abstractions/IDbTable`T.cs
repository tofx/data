using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TOF.Data.Abstractions
{
    public interface IDbTable<TModel> : IEnumerable<TModel> where TModel: class, new()
    {
        IDbModelStrategy ModelStrategy { get; }
        bool IsView { get; }
        void Insert(TModel model);
        void Insert(IEnumerable<TModel> models);
        void Update(TModel model);
        void Update(IEnumerable<TModel> models);
        void Delete(TModel model);
        void Delete(IEnumerable<TModel> models);
        void Empty();
        void DeleteAll();
        IDbQueryContext<TModel> Join<TJoinModel>(
             IDbTable<TJoinModel> table,
             Expression<Func<TModel, object>> innerSpecifierExpression,
             Expression<Func<TModel, object>> outerSpecifierExpression) where TJoinModel : class, new();
        IDbQueryContext<TModel> Where(Expression<Func<TModel, object>> specifierExpression, DbQueryConditionOperators Operator = DbQueryConditionOperators.And);
        IDbQueryContext<TModel> OrderBy(Expression<Func<TModel, object>> specifierExpression);
        IDbQueryContext<TModel> OrderByDescending(Expression<Func<TModel, object>> specifierExpression);
        
        #region Aggregation Operators
        bool Any(Expression<Func<TModel, object>> specifierExpression);
        int Count(Expression<Func<TModel, object>> specifierExpression, int defaultValue = 0);
        long CountLong(Expression<Func<TModel, object>> specifierExpression, long defaultValue = 0);
        decimal Average(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);
        decimal Max(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);
        decimal Min(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);
        decimal Sum(Expression<Func<TModel, object>> columnSpecifier, Expression<Func<TModel, object>> specifierExpression = null, Expression<Func<TModel, object>> groupByExpression = null, decimal defaultValue = 0);
        #endregion
    }
}
