using System;
using System.Linq.Expressions;

namespace tofx.Data.Abstractions
{
    public interface IDbModelStrategy<TModel> : IDbModelStrategy where TModel: class, new()
    {
        IDbModelPropertyStrategy Property(Expression<Func<TModel, object>> propertySpecifier);
        IDbStoredProcedureStrategy<TModel> InsertProc(string procedureName);
        IDbStoredProcedureStrategy<TModel> UpdateProc(string procedureName);
        IDbStoredProcedureStrategy<TModel> DeleteProc(string procedureName);
    }
}
