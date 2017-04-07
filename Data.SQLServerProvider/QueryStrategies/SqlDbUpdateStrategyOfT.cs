using tofx.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.QueryStrategies
{
    public class SqlDbUpdateStrategy<TModel> : DbQueryStrategyBase<TModel>, IDbQueryValueIncreDecreStrategy
        where TModel : class, new()
    {
        public SqlDbUpdateStrategy() : base(DbQueryStrategyTypes.Update)
        {
        }

        public SqlDbUpdateStrategy(IDbModelStrategy modelStrategy) : base(modelStrategy, DbQueryStrategyTypes.Update)
        {
        }

        public override string GetDbQueryScript()
        {
            ParameterChecker.NotNull(base.ModelStrategy);

            var propBindings = base.ModelStrategy.PropertyStrategies;

            if (!propBindings.Where(c => c.IsKey()).Any())
                throw new InvalidOperationException("ModelKeyNotFound");

            var updateQueryBuilder = new StringBuilder();
            var updateColumnBuilder = new StringBuilder();
            var updateClausesBuilder = new StringBuilder();
            var updateColumns = new List<string>();
            var updateClauses = new List<string>();

            updateQueryBuilder.Append("UPDATE [");
            updateQueryBuilder.Append(base.ModelStrategy.DbTableName);
            updateQueryBuilder.Append("] SET ");
            
            foreach (var column in base.Columns)
            {
                var propNameQuery = propBindings.Where(c => c.GetParameterName() == column);

                if (propNameQuery.Any())
                {
                    if (propNameQuery.First().IsKey())
                    {
                        updateClauses.Add(string.Format("{0}=@{1}",
                            column, propNameQuery.First().GetParameterName()));
                    }
                    else
                    {
                        updateColumns.Add(string.Format("{0}=@{1}",
                            column, propNameQuery.First().GetParameterName()));
                    }

                    continue;
                }

                var propPropertyInfoQuery = propBindings.Where(c => c.GetPropertyInfo().Name == column);

                if (propPropertyInfoQuery.Any())
                {
                    if (propPropertyInfoQuery.First().IsKey())
                    {
                        updateClauses.Add(string.Format("{0}=@{1}",
                            column, propNameQuery.First().GetParameterName()));
                    }
                    else
                    {
                        updateColumns.Add(string.Format("{0}=@{1}",
                            column, propNameQuery.First().GetParameterName()));
                    }

                    continue;
                }

                updateColumns.Add(string.Format("{0}=@{1}",
                    column, propNameQuery.First().GetParameterName()));
            }

            updateColumnBuilder.Append(string.Join(", ", 
                updateColumns.Where(c => !string.IsNullOrEmpty(c)).ToArray()));
            updateClausesBuilder.Append(string.Join(" AND ", 
                updateClauses.Where(c => !string.IsNullOrEmpty(c)).ToArray()));

            updateQueryBuilder.Append(updateColumnBuilder.ToString());
            updateQueryBuilder.Append(" WHERE ");            
            updateQueryBuilder.Append(updateClausesBuilder);

            return updateQueryBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> GetDbParameters()
        {
            var propBindings = ModelStrategy.PropertyStrategies;

            if (!propBindings.Where(c => c.IsKey()).Any())
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

        public string GetDbValueIncreaseQuery()
        {
            return RenderIncreaseDecreaseUpdateQueryInternal(true);
        }

        public IEnumerable<IDbDataParameter> GetDbIncreaseQueryParameters()
        {
            var keyBindings = ModelStrategy.PropertyStrategies.Where(c => c.IsKey());

            if (!keyBindings.Any())
                throw new InvalidOperationException("ERROR_MODEL_KEY_NOT_FOUND");

            foreach (var propBinding in keyBindings)
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

        public string GetDbValueDecreaseQuery()
        {
            return RenderIncreaseDecreaseUpdateQueryInternal(false);
        }

        public IEnumerable<IDbDataParameter> GetDbDecreaseQueryParameters()
        {
            var keyBindings = ModelStrategy.PropertyStrategies.Where(c => c.IsKey());

            if (!keyBindings.Any())
                throw new InvalidOperationException("ERROR_MODEL_KEY_NOT_FOUND");

            foreach (var propBinding in keyBindings)
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

        public bool SupportIncrease()
        {
            if (ModelStrategy == null)
                return false;

            return ModelStrategy.PropertyStrategies.Where(c => c.IsIncremental()).Any();
        }

        public bool SupportDecrease()
        {
            if (ModelStrategy == null)
                return false;

            return ModelStrategy.PropertyStrategies.Where(c => c.IsIncremental()).Any();
        }

        private string RenderIncreaseDecreaseUpdateQueryInternal(bool isIncrease)
        {
            ParameterChecker.NotNull(base.ModelStrategy);

            if (isIncrease)
            {
                if (!SupportIncrease())
                    throw new NotSupportedException("ModelStrategyNotSupportIncreaseOperation");
            }
            else
            {
                if (!SupportDecrease())
                    throw new NotSupportedException("ModelStrategyNotSupportDecreaseOperation");
            }

            var propBindings = base.ModelStrategy.PropertyStrategies;

            if (!propBindings.Where(c => c.IsKey()).Any())
                throw new InvalidOperationException("ModelKeyNotFound");

            var updateQueryBuilder = new StringBuilder();
            var updateColumnBuilder = new StringBuilder();
            var updateClausesBuilder = new StringBuilder();

            updateQueryBuilder.Append("UPDATE ");
            updateQueryBuilder.Append(base.ModelStrategy.DbTableName);
            updateQueryBuilder.Append(" SET ");

            var incrementalColumns = propBindings.Where(c => c.IsIncremental());

            if (!incrementalColumns.Any())
                return null;

            // create incremental fields.
            foreach (var f in incrementalColumns)
            {
                if (updateColumnBuilder.Length > 0)
                    updateColumnBuilder.Append(", ");

                updateColumnBuilder.Append(f.GetParameterName());
                updateColumnBuilder.Append(" = ");
                updateColumnBuilder.Append(f.GetParameterName());

                if (isIncrease)
                    updateColumnBuilder.Append(" + 1 ");
                else
                    updateColumnBuilder.Append(" - 1 ");
            }

            // create key fields.
            var keyFields = propBindings.Where(c => c.IsKey());

            foreach (var f in keyFields)
            {
                updateClausesBuilder.Append(f.GetParameterName());
                updateClausesBuilder.Append(" = ");
                updateClausesBuilder.Append("@" + f.GetParameterName());
            }

            updateQueryBuilder.Append(updateColumnBuilder.ToString());
            updateQueryBuilder.Append(" WHERE ");
            updateQueryBuilder.Append(updateClausesBuilder);

            return updateQueryBuilder.ToString();
        }
    }
}
