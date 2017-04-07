using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.Expressions;

namespace TOF.Data.Providers.SqlServer
{
    public class SqlDbQueryExpressionParserProvider : IDbQueryExpressionParserProvider
    {
        public IDbQueryExpressionParser GetExpressionParser()
        {
            return new SqlDbQueryExpressionParser();
        }
    }
}
