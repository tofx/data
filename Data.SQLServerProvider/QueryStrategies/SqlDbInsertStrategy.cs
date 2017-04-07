using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using tofx.Core.Utils;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbInsertStrategy : DbQueryStrategyBase
    {
        public SqlDbInsertStrategy(Type modelType)
            : base(modelType)
        {
        }

        public SqlDbInsertStrategy(IDbModelStrategy modelStrategy)
            : base(modelStrategy)
        {
        }

        public override string GetDbQueryScript()
        {
            ParameterChecker.NotNull(base.ModelStrategy);

            var insertQueryBuilder = new StringBuilder();
            var insertColumnBuilder = new StringBuilder();
            var insertFieldBuilder = new StringBuilder();

            insertQueryBuilder.Append("INSERT INTO [");
            insertQueryBuilder.Append(base.ModelStrategy.DbTableName);
            insertQueryBuilder.Append("] (");
            
            foreach (var column in base.Columns)
            {
                if (insertColumnBuilder.Length == 0)
                    insertColumnBuilder.Append(column);
                else
                    insertColumnBuilder.Append(", ").Append(column);
            }

            insertQueryBuilder.Append(insertColumnBuilder.ToString());
            insertQueryBuilder.Append(") VALUES (");

            var propBindings = base.ModelStrategy.PropertyStrategies;

            foreach (var column in base.Columns)
            {
                if (insertFieldBuilder.Length > 0)
                    insertFieldBuilder.Append(", ");

                var propNameQuery = propBindings.Where(c => c.GetParameterName() == column);

                if (propNameQuery.Any())
                {
                    insertFieldBuilder.Append("@" + propNameQuery.First().GetParameterName());
                    continue;
                }

                var propPropertyInfoQuery = propBindings.Where(c => c.GetPropertyInfo().Name == column);

                if (propPropertyInfoQuery.Any())
                {
                    insertFieldBuilder.Append("@" + propPropertyInfoQuery.First().GetPropertyInfo().Name);
                    continue;
                }

                insertFieldBuilder.Append("@" + column);
            }

            insertQueryBuilder.Append(insertFieldBuilder.ToString());
            insertQueryBuilder.Append(")");

            return insertQueryBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> GetDbParameters()
        {
            var propBindings = ModelStrategy.PropertyStrategies;

            foreach (var propBinding in propBindings)
            {
                SqlParameter param = new SqlParameter();

                param.ParameterName = "@" + propBinding.GetParameterName();

                if (propBinding.GetMapDbType() != null)
                    param.DbType = propBinding.GetMapDbType().Value;
                if (propBinding.GetLength() != null)
                    param.Size = propBinding.GetLength().Value;

                yield return param;
            }
        }
    }
}
