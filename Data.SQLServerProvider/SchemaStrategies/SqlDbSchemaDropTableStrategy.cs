﻿using tofx.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.SchemaStrategies
{
    public class SqlDbSchemaDropTableStrategy : IDbSchemaStrategy
    {
        protected IDbModelStrategy ModelStrategy = null;

        public SqlDbSchemaDropTableStrategy(IDbModelStrategy modelStrategy)
        {
            ParameterChecker.NotNull(modelStrategy);
            this.ModelStrategy = modelStrategy;
        }

        public DbSchemaStrategyTypes Type { get { return DbSchemaStrategyTypes.Drop; } }

        public string GetDbSchemaScript()
        {
            string tableName = ModelStrategy.DbTableName;
            return "DROP TABLE " + tableName;
        }
    }
}
