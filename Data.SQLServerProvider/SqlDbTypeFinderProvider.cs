using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.MapType;

namespace TOF.Data.Providers.SqlServer
{
    public class SqlDbTypeFinderProvider : IDbTypeFinderProvider
    {
        public IDbTypeFinder GetDbTypeFinder()
        {
            return new SqlDbTypeFinder();
        }
    }
}
