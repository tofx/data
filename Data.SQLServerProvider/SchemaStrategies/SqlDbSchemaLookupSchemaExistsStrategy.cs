using tofx.Core.Utils;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.SchemaStrategies
{
    public class SqlDbSchemaLookupSchemaExistsStrategy : IDbSchemaStrategy
    {
        protected IDbModelStrategy ModelStrategy = null;

        public SqlDbSchemaLookupSchemaExistsStrategy(IDbModelStrategy modelStrategy)
        {
            ParameterChecker.NotNull(modelStrategy);
            this.ModelStrategy = modelStrategy;
        }

        public DbSchemaStrategyTypes Type { get { return DbSchemaStrategyTypes.LookupSchemaExists; } }

        public string GetDbSchemaScript()
        {
            return string.Format("SELECT * FROM sys.tables WHERE name = '{0}'", ModelStrategy.DbTableName);
        }
    }
}
