using System;

namespace TOF.Data.Annotations
{
    /// <summary>
    /// Mark the property as auto-incremental column in database.
    /// FOR INT/BIGINT/SMALLINT USAGE ONLY.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class AutoIncrementalAttribute : Attribute
    {
    }
}
