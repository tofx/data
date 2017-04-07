using System.Collections;
using System.Collections.Generic;

namespace TOF.Data.Abstractions
{
    public interface IDbTable : IDbObject, IEnumerable
    {
        IDbModelStrategy ModelStrategy { get; }
        bool IsView { get; }
        void Insert<TModel>(TModel model) where TModel : class, new();
        void Insert<TModel>(IEnumerable<TModel> models) where TModel : class, new();
        void Update<TModel>(TModel model) where TModel : class, new();
        void Update<TModel>(IEnumerable<TModel> models) where TModel : class, new();
        void Delete<TModel>(TModel model) where TModel : class, new();
        void Delete<TModel>(IEnumerable<TModel> models) where TModel : class, new();
        void Empty();
        void DeleteAll();
    }
}
