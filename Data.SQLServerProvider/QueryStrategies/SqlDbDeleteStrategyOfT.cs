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
    public class SqlDbDeleteStrategy<TModel> : DbQueryStrategyBase<TModel>
        where TModel : class, new()
    {
        public SqlDbDeleteStrategy() : base(DbQueryStrategyTypes.Delete)
        {
        }

        public SqlDbDeleteStrategy(IDbModelStrategy modelStrategy) : base(modelStrategy, DbQueryStrategyTypes.Delete)
        {
        }

        public override string GetDbQueryScript()
        {
            ParameterChecker.NotNull(base.ModelStrategy);

            var propBindings = base.ModelStrategy.PropertyStrategies;

            if (!propBindings.Where(c => c.IsKey()).Any())
                throw new InvalidOperationException("ModelKeyNotFound");

            var deleteQueryBuilder = new StringBuilder();
            var deleteClausesBuilder = new StringBuilder();

            deleteQueryBuilder.Append("DELETE FROM [");
            deleteQueryBuilder.Append(base.ModelStrategy.DbTableName);
            deleteQueryBuilder.Append("] ");

            foreach (var column in base.Columns)
            {
                var propNameQuery = propBindings.Where(c => c.GetParameterName() == column);

                if (propNameQuery.Any())
                {
                    if (propNameQuery.First().IsKey())
                    {
                        if (deleteClausesBuilder.Length > 0)
                            deleteClausesBuilder.Append(" AND ");

                        deleteClausesBuilder.Append(column);
                        deleteClausesBuilder.Append(" = ");
                        deleteClausesBuilder.Append("@" + propNameQuery.First().GetParameterName());
                    }

                    continue;
                }

                var propPropertyInfoQuery = propBindings.Where(c => c.GetPropertyInfo().Name == column);

                if (propPropertyInfoQuery.Any())
                {
                    if (propPropertyInfoQuery.First().IsKey())
                    {
                        if (deleteClausesBuilder.Length > 0)
                            deleteClausesBuilder.Append(" AND ");

                        deleteClausesBuilder.Append(column);
                        deleteClausesBuilder.Append(" = ");
                        deleteClausesBuilder.Append("@" + propPropertyInfoQuery.First().GetPropertyInfo().Name);
                    }

                    continue;
                }
            }

            deleteQueryBuilder.Append(" WHERE ");
            deleteQueryBuilder.Append(deleteClausesBuilder);

            return deleteQueryBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> GetDbParameters()
        {
            var propBindings = ModelStrategy.PropertyStrategies.Where(c => c.IsKey());

            if (!propBindings.Any())
                throw new InvalidOperationException("ModelKeyNotFound");

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
