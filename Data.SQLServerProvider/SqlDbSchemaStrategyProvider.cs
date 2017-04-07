using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.SchemaStrategies;

namespace TOF.Data.Providers.SqlServer
{
    public class SqlDbSchemaStrategyProvider : IDbSchemaStrategyProvider
    {
        public IDbSchemaStrategy GetAlterTableStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbSchemaAlterTableStrategy(modelStrategy);
        }

        public IDbSchemaStrategy GetCreateTableStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbSchemaCreateTableStrategy(modelStrategy);
        }

        public IDbSchemaStrategy GetDropTableStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbSchemaDropTableStrategy(modelStrategy);
        }

        public IDbSchemaStrategy GetLookupSchemaExistsStrategy(IDbModelStrategy modelStrategy)
        {
            return new SqlDbSchemaLookupSchemaExistsStrategy(modelStrategy);
        }
    }
}
