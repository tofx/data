﻿using tofx.Core.DependencyInjection;
using tofx.Core.Utils;
using System.Collections.Generic;
using tofx.Data.Abstractions;

[assembly: Startup(typeof(tofx.Data.Schema.Startup))]

namespace tofx.Data.Schema
{
    public class Startup
    {
        public void Initialize(ContainerBuilder builder)
        {
            builder.Register<DbSchemaActionProvider>()
                .As<IDbSchemaActionProvider>()
                .OnActivating((c, p) =>
                {
                    if (p.Named<IEnumerable<IDbSchemaStrategy>>("SchemaDefinitionStrategies") != null)
                    {
                        return new DbSchemaActionProvider(
                            p.Named<IDbModelStrategy>("ModelStrategy"),
                            p.Named<IEnumerable<IDbSchemaStrategy>>("SchemaDefinitionStrategies"),
                            p.Named<string>("ConnectionString"));
                    }
                    else
                    {
                        var strategyProvider = c.Resolve<IDbSchemaStrategyProvider>();

                        ParameterChecker.NotNull(strategyProvider);

                        List<IDbSchemaStrategy> strategies = new List<IDbSchemaStrategy>();
                        strategies.Add(strategyProvider.GetCreateTableStrategy(p.Named<IDbModelStrategy>("ModelStrategy")));
                        strategies.Add(strategyProvider.GetAlterTableStrategy(p.Named<IDbModelStrategy>("ModelStrategy")));
                        strategies.Add(strategyProvider.GetDropTableStrategy(p.Named<IDbModelStrategy>("ModelStrategy")));
                        strategies.Add(strategyProvider.GetLookupSchemaExistsStrategy(p.Named<IDbModelStrategy>("ModelStrategy")));

                        return new DbSchemaActionProvider(
                            p.Named<IDbModelStrategy>("ModelStrategy"),
                            strategies,
                            p.Named<string>("ConnectionString"));
                    }
                });
        }
    }
}
