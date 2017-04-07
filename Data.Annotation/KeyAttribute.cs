using System;

namespace TOF.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute
    {
    }
}
