using tofx.Data.Abstractions;
using System;

namespace tofx.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
        public DbIgnorePropertyKinds IgnoreKinds { get; set; }

        public IgnoreAttribute()
        {
            IgnoreKinds = DbIgnorePropertyKinds.InsertAndUpdate;
        }

        public IgnoreAttribute(DbIgnorePropertyKinds ignoreKinds)
        {
            IgnoreKinds = ignoreKinds;
        }
    }
}
