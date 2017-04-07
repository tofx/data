using System;
using System.Data;

namespace tofx.Data.Abstractions
{
    public interface IDbTypeMap
    {
        int DbTypeNumber { get; }
        DbType DbType { get; }
        Type ClrType { get; }
    }

}
