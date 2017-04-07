using tofx.Data.Abstractions;
using tofx.Data.Providers.SqlServer.MapType;

namespace tofx.Data.Providers.SqlServer
{
    public class SqlDbTypeFinderProvider : IDbTypeFinderProvider
    {
        public IDbTypeFinder GetDbTypeFinder()
        {
            return new SqlDbTypeFinder();
        }
    }
}
