using System;

namespace tofx.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultValueAttribute : Attribute
    {
        public object DefaultValue { get; private set; }
        public DefaultValueAttribute(object DefaultValue)
        {
            this.DefaultValue = DefaultValue;
        }
    }
}
