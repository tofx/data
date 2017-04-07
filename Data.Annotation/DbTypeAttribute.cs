using System;
using System.Data;

namespace TOF.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DbTypeAttribute : Attribute
    {
        public DbType DbType { get; private set; }

        public DbTypeAttribute(DbType Type)
        {
            this.DbType = Type;
        }
    }
}
