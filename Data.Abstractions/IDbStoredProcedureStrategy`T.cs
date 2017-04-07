using System;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public interface IDbStoredProcedureStrategy<TModel> : IDbStoredProcedureStrategy where TModel: class, new()
    {
        IDbModelPropertyStrategy Property(string parameterName, Expression<Func<TModel, object>> propertySpecifier);
    }
}
