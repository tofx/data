using System.Data;
using System.Reflection;

namespace TOF.Data.Abstractions
{
    public interface IDbParameterParser
    {
        IDbModelPropertyStrategy GetModelPropertyStrategy();
        PropertyInfo GetPropertyInfo();
        IDbDataParameter GetDbParameter();

    }
}