using System;
using System.Collections.Generic;
using System.Reflection;

namespace TOF.Data.Abstractions
{
    public interface IDbModelStrategy
    {
        Type ModelType { get; }
        string DbTableName { get; }
        void ChangeTableName(string tableName, string tableAlias = null);
        IEnumerable<IDbModelPropertyStrategy> PropertyStrategies { get; }
        IDbModelPropertyStrategy GetPropertyStrategy(PropertyInfo property);
    }
}
