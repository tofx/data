using TOF.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace TOF.Data
{
    // TODO: implement ILogRequriedException interface.
    public class DbMultipleOperationsException : Exception, ILoggableException
    {
        private readonly IEnumerable<DbOperationException> _operationExceptions;

        public DbMultipleOperationsException(IEnumerable<DbOperationException> operationExceptions)
            : base("ERROR_MULTI_OPERATIONS_OCCUR_EXCEPTION")
        {
            _operationExceptions = operationExceptions;
        }

        public IEnumerable<DbOperationException> GetOperationExceptions()
        {
            return _operationExceptions;
        }

        public string WriteLog(ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
