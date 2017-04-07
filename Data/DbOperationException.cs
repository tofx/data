using TOF.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;

namespace TOF.Data
{
    // TODO: implement ILogRequriedException interface 
    public class DbOperationException : Exception, ILoggableException
    {
        private string _exceptionStatement = null;
        private IEnumerable<IDbDataParameter> _exceptionParameters = null;

        public DbOperationException(string message, string statement, IEnumerable<IDbDataParameter> parameters)
            : base(message)
        {
        }

        public DbOperationException(string message, Exception innerException, string statement, IEnumerable<IDbDataParameter> parameters)
            : base(message, innerException)
        {
            _exceptionStatement = statement;
            _exceptionParameters = parameters;
        }

        public string GetExceptionStatement()
        {
            return _exceptionStatement;
        }

        public IEnumerable<IDbDataParameter> GetExceptionParameters()
        {
            return _exceptionParameters;
        }

        public string WriteLog(ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
