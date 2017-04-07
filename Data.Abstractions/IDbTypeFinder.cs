using System;
using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbTypeFinder
    {
        bool FindByTypeNumber(int dbTypeNumber, out DbType dbType);
        bool FindByTypeNumber(int dbTypeNumber, out Type clrType);
        bool FindByTypeNumber(int dbTypeNumber, bool returnDefaultOnly, out DbType dbType);
        bool FindByDbType(DbType dbType, out int typeNumber);
        bool FindByDbType(DbType dbType, out Type clrType);
        bool FindByDbTypeNumber(int dbTypeNumber, out object dbType);
        bool FindByClrType(Type clrType, out DbType dbType);
        bool FindByClrType(Type clrType, out int typeNumber);
    }
}
