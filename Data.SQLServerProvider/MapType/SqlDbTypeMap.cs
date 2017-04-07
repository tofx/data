using System;
using System.Data;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.MapType
{
    public class SqlDbTypeMap : IDbTypeMap
    {
        public int DbTypeNumber { get; internal set; }
        public SqlDbType SqlDbType { get; internal set; }
        public DbType DbType { get; internal set; }
        public Type ClrType { get; internal set; }
        public bool Default { get; internal set; }
    }
}
