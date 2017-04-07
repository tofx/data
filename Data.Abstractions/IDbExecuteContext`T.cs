using System.Collections.Generic;

namespace TOF.Data.Abstractions
{
    public interface IDbExecuteContext<TModel> where TModel: class, new()
    {
        void Insert(TModel model);
        void Update(TModel model);
        void Delete(TModel model);
        int Insert(IEnumerable<TModel> models, bool throwIfAnyModelFailed = true);
        int Update(IEnumerable<TModel> models, bool throwIfAnyModelFailed = true);
        int Delete(IEnumerable<TModel> models, bool throwIfAnyModelFailed = true);
    }
}
