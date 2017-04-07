using System;
using System.Collections.Generic;

namespace tofx.Data.Abstractions
{
    public class DbAggregateException : Exception
    {
        public IDictionary<int, Exception> Exceptions { get; private set; }

        public DbAggregateException(Exception exception) : base("One or more database exception occurred")
        {
            Exceptions = new Dictionary<int, Exception>() { { 0, exception } };
        }

        public DbAggregateException(IDictionary<int, Exception> exceptions) : base("One or more database exception occurred")
        {
            Exceptions = exceptions;
        }
    }
}
