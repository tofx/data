using tofx.Core.Utils;
using System;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.SchemaStrategies
{
    public class SqlDbSchemaAlterTableStrategy : IDbSchemaStrategy
    {
        public DbSchemaStrategyTypes Type { get { return DbSchemaStrategyTypes.Alter; } }

        protected IDbModelStrategy ModelStrategy = null;

        public SqlDbSchemaAlterTableStrategy(IDbModelStrategy modelStrategy)
        {
            ParameterChecker.NotNull(modelStrategy);
            this.ModelStrategy = modelStrategy;
        }

        public string GetDbSchemaScript()
        {
            throw new NotImplementedException();
        }
    }
}
