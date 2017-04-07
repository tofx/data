using System;
using System.Data;

namespace TOF.Data.Abstractions
{
    public interface IDbTypeMap
    {
        int DbTypeNumber { get; }
        DbType DbType { get; }
        Type ClrType { get; }
    }

}
