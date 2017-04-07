using System;

namespace tofx.Data.Abstractions
{
    public class DbKeyNotAllowNullException : Exception
    {
        public DbKeyNotAllowNullException() : base("Key is not allowed NULL") { }
    }
}
