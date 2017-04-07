using tofx.Core.Utils;
using System;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbEmptyStrategy : DbQueryStrategyBase
    {
        public SqlDbEmptyStrategy(Type modelType) : base(modelType)
        {
        }

        public SqlDbEmptyStrategy(IDbModelStrategy modelStrategy) : base(modelStrategy)
        {
        }

        public override string GetDbQueryScript()
        {
            ParameterChecker.NotNull(ModelStrategy);

            var tableName = ModelStrategy.DbTableName;
            var emptySql = new StringBuilder();

            emptySql.Append("TRUNCATE TABLE [");
            emptySql.Append(tableName);
            emptySql.Append("]");

            return emptySql.ToString();
        }
    }
}
