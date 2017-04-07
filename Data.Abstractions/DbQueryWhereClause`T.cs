using System;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public class DbQueryWhereClause<TModel> where TModel: class, new()
    {
        public Expression<Func<TModel, object>> Clause { get; set; }
        public DbQueryConditionOperators Operator { get; set; }
    }
}
