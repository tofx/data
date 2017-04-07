using System;
using System.Linq.Expressions;

namespace TOF.Data.Abstractions
{
    public class DbQueryOrderByClause<TModel> where TModel: class, new()
    {
        public Expression<Func<TModel, object>> OrderByClause { get; set; }
        public DbQueryOrderByOperators Operator { get; set; }
    }
}
