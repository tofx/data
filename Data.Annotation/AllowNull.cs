using System;

namespace tofx.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowNullAttribute : Attribute
    {
        public bool AllowNull { get; private set; }

        public AllowNullAttribute()
        {
            this.AllowNull = true;
        }
        public AllowNullAttribute(bool AllowNull)
        {
            this.AllowNull = AllowNull;
        }
    }
}
