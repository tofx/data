using System.Data;
using System.Reflection;

namespace tofx.Data.Abstractions
{
    public interface IDbParameterParser
    {
        IDbModelPropertyStrategy GetModelPropertyStrategy();
        PropertyInfo GetPropertyInfo();
        IDbDataParameter GetDbParameter();

    }
}