using tofx.Data.Abstractions;
using tofx.Data.Providers.SqlServer.Expressions;

namespace tofx.Data.Providers.SqlServer
{
    public class SqlDbQueryExpressionParserProvider : IDbQueryExpressionParserProvider
    {
        public IDbQueryExpressionParser GetExpressionParser()
        {
            return new SqlDbQueryExpressionParser();
        }
    }
}
