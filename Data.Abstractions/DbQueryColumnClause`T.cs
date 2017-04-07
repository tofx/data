using System;
using System.Linq.Expressions;

namespace TOF.Data.Abstractions
{
    public class DbQueryColumnClause<TModel> where TModel : class, new()
    {
        public Expression<Func<TModel, object>> ColumExpression { get; set; }
    }
}
