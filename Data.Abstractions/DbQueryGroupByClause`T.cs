﻿using System;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public class DbQueryGroupByClause<TModel> where TModel: class, new()
    {
        public Expression<Func<TModel, object>> Clause { get; set; }
    }
}
