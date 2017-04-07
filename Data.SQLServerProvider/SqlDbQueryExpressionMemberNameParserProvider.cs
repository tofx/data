using System;
using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.Expressions;

namespace TOF.Data.Providers.SqlServer
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