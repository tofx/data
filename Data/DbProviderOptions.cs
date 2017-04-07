using tofx.Data.Abstractions;

namespace tofx.Data
{
    public class DbProviderOptions : IDbProviderOptions
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringName { get; set; }

        public DbProviderOptions()
        {
            ConnectionStringName = "default";
        }
    }
}
