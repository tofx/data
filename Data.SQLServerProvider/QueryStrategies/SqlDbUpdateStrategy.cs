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
    public class SqlDbUpdateStrategy : DbQueryStrategyBase, IDbQueryValueIncreDecreStrategy
    {
        public SqlDbUpdateStrategy(Type modelType)
            : base(modelType)
        {
        }

        public SqlDbUpdateStrategy(IDbModelStrategy modelStrategy) : base(modelStrategy)
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

            updateQueryBuilder.Append("UPDATE [");
            updateQueryBuilder.Append(base.ModelStrategy.DbTableName);
            updateQueryBuilder.Append("] SET ");
            
            foreach (var column in base.Columns)
            {
                if (updateColumnBuilder.Length > 0)
                    updateColumnBuilder.Append(", ");

                var propNameQuery = propBindings.Where(c => c.GetParameterName() == column);

                if (propNameQuery.Any())
                {
                    updateColumnBuilder.Append(column);
                    updateColumnBuilder.Append(" = ");
                    updateColumnBuilder.Append("@" + propNameQuery.First().GetParameterName());

                    if (propNameQuery.First().IsKey())
                    {
                        if (updateClausesBuilder.Length > 0)
                            updateClausesBuilder.Append(" AND ");

                        updateClausesBuilder.Append(column);
                        updateClausesBuilder.Append(" = ");
                        updateClausesBuilder.Append("@" + propNameQuery.First().GetParameterName());
                    }

                    continue;
                }

                var propPropertyInfoQuery = propBindings.Where(c => c.GetPropertyInfo().Name == column);

                if (propPropertyInfoQuery.Any())
                {
                    updateColumnBuilder.Append(column);
                    updateColumnBuilder.Append(" = ");
                    updateColumnBuilder.Append("@" + propPropertyInfoQuery.First().GetPropertyInfo().Name);

                    if (propPropertyInfoQuery.First().IsKey())
                    {
                        if (updateClausesBuilder.Length > 0)
                            updateClausesBuilder.Append(" AND ");

                        updateClausesBuilder.Append(column);
                        updateClausesBuilder.Append(" = ");
                        updateClausesBuilder.Append("@" + propPropertyInfoQuery.First().GetPropertyInfo().Name);
                    }

                    continue;
                }

                updateColumnBuilder.Append(column);
                updateColumnBuilder.Append(" = ");
                updateColumnBuilder.Append("@" + column);
            }

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
            return GetDbIncreaseDecreaseUpdateQueryInternal(true);
        }

        public IEnumerable<IDbDataParameter> GetDbIncreaseQueryParameters()
        {
            var keyBindings = ModelStrategy.PropertyStrategies.Where(c => c.IsKey());

            if (!keyBindings.Any())
                throw new InvalidOperationException("ModelNotFound");

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
            return GetDbIncreaseDecreaseUpdateQueryInternal(false);
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

        private string GetDbIncreaseDecreaseUpdateQueryInternal(bool isIncrease)
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
