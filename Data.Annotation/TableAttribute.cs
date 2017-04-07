using System;

namespace TOF.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; private set; }

        public TableAttribute(string TableName)
        {
            this.TableName = TableName;
        }
    }
}
