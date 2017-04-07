using System;
using System.Linq.Expressions;

namespace TOF.Data.Abstractions
{
    public interface IDbStoredProcedureStrategy<TModel> : IDbStoredProcedureStrategy where TModel: class, new()
    {
        IDbModelPropertyStrategy Property(string parameterName, Expression<Func<TModel, object>> propertySpecifier);
    }
}
