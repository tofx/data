using tofx.Core.Abstractions;
using System;

namespace tofx.Data
{
    public class DbEnvironmentException : Exception, ILoggableException
    {
        public DbEnvironmentException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }

        public string WriteLog(ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
