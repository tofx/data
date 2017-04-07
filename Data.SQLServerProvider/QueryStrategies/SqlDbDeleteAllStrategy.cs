using TOF.Core.Utils;
using System;
using System.Text;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbDeleteAllStrategy : DbQueryStrategyBase
    {
        public SqlDbDeleteAllStrategy(Type modelType) : base(modelType)
        {
        }

        public SqlDbDeleteAllStrategy(IDbModelStrategy modelStrategy) : base(modelStrategy)
        {
        }

        public override string GetDbQueryScript()
        {
            ParameterChecker.NotNull(ModelStrategy);

            var tableName = ModelStrategy.DbTableName;
            var deleteAllSql = new StringBuilder();

            deleteAllSql.Append("DELETE FROM [");
            deleteAllSql.Append(tableName);
            deleteAllSql.Append("]");

            return deleteAllSql.ToString();
        }
    }
}
