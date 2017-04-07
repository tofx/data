using System.Collections.Generic;

namespace tofx.Data.Abstractions
{
    public interface IDbStoredProcedureInvoker<in TModel> where TModel: class, new()
    {
        void Invoke(TModel model);
        void Invoke(IEnumerable<TModel> models);
        IEnumerable<dynamic> InvokeGet(TModel model);
    }
}
