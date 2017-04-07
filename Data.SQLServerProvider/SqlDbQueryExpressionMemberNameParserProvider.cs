using System;
using tofx.Data.Abstractions;
using tofx.Data.Providers.SqlServer.Expressions;

namespace tofx.Data.Providers.SqlServer
{
    public class SqlDbQueryExpressionMemberNameParserProvider : IDbQueryExpressionMemberNameParserProvider
    {
        public IDbQueryExpressionMemberNameParser GetParser(IDbModelStrategy modelStrategy = null)
        {
            return new SqlDbQueryGetMemberNameExpressionNode()
            {
                ModelStrategy = modelStrategy
            };
        }
    }
}