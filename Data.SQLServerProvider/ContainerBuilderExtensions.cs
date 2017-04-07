using tofx.Core.Configuration;
using tofx.Core.DependencyInjection;
using tofx.Core.Infrastructure;
using tofx.Data.Abstractions;
using tofx.Data.Providers.SqlServer.Expressions;
using tofx.Data.Providers.SqlServer.InspectorStrategies;
using System;

namespace tofx.Data.Providers.SqlServer
{
    public static class ContainerBuilderExtensions
    {
        public static void AddSqlServerDbProvider(this ContainerBuilder builder)
        {
            IConfiguration configruration = App.Configuration;
            var options = new DbProviderOptions();

            // SQL Server Runtime Provider.
            builder.Register<SqlDbServiceProvider>()
                .As<IDbServiceProvider>()
                .As<IDbTransactionProvider>()
                .OnActivating((c, p) =>
                {
                    if (!string.IsNullOrEmpty(p.Named<string>("ConnectionString")))
                        return new SqlDbServiceProvider(p.Named<string>("ConnectionString"));
                    else if (!string.IsNullOrEmpty(options.ConnectionStringName))
                        return new SqlDbServiceProvider(configruration["connectionStrings/" + options.ConnectionStringName]);

                    throw new ArgumentException("Database Connection String is not initialized.");
                });

            builder.Register<SqlDbQueryStrategyProvider>().As<IDbQueryStrategyProvider>();
            builder.Register<SqlDbParamChainProvider>().As<IDbParamChainProvider>().AsSingleInstance();
            builder.Register<SqlDbQueryExpressionMemberNameParserProvider>().As<IDbQueryExpressionMemberNameParserProvider>().AsSingleInstance();
            builder.Register<SqlDbQueryExpressionParserProvider>().As<IDbQueryExpressionParserProvider>().AsSingleInstance();
            builder.Register<SqlDbTypeFinderProvider>().As<IDbTypeFinderProvider>().AsSingleInstance();
            builder.Register<SqlDbSchemaStrategyProvider>().As<IDbSchemaStrategyProvider>();
            builder.Register<SqlDbQueryAddExpressionNode>().As<IDbQueryExpressionNode>();
            builder.Register<SqlDbEnumTableInspectorStrategy>().As<IDbEnumTableInspectorStrategy>();
            builder.Register<SqlDbTableInspectorStrategy>().As<IDbTableInspectorStrategy>();
        }

        public static void AddSqlServerDbProvider(this ContainerBuilder builder, IDbProviderOptions options)
        {
            IConfiguration configruration = App.Configuration;

            // SQL Server Runtime Provider.
            builder.Register<SqlDbServiceProvider>()
                .As<IDbServiceProvider>()
                .As<IDbTransactionProvider>()
                .OnActivating((c, p) =>
                {
                    if (!string.IsNullOrEmpty(p.Named<string>("ConnectionString")))
                        return new SqlDbServiceProvider(p.Named<string>("ConnectionString"));
                    else if(!string.IsNullOrEmpty(options.ConnectionString))
                        return new SqlDbServiceProvider(options.ConnectionString);
                    else if (!string.IsNullOrEmpty(options.ConnectionStringName))
                        return new SqlDbServiceProvider(configruration["connectionStrings/" + options.ConnectionStringName]);

                    throw new ArgumentException("Database Connection String is not initialized.");
                });

            builder.Register<SqlDbQueryStrategyProvider>().As<IDbQueryStrategyProvider>();
            builder.Register<SqlDbParamChainProvider>().As<IDbParamChainProvider>().AsSingleInstance();
            builder.Register<SqlDbQueryExpressionMemberNameParserProvider>().As<IDbQueryExpressionMemberNameParserProvider>().AsSingleInstance();
            builder.Register<SqlDbQueryExpressionParserProvider>().As<IDbQueryExpressionParserProvider>().AsSingleInstance();
            builder.Register<SqlDbTypeFinderProvider>().As<IDbTypeFinderProvider>().AsSingleInstance();
            builder.Register<SqlDbSchemaStrategyProvider>().As<IDbSchemaStrategyProvider>();
            builder.Register<SqlDbQueryAddExpressionNode>().As<IDbQueryExpressionNode>();
            builder.Register<SqlDbEnumTableInspectorStrategy>().As<IDbEnumTableInspectorStrategy>();
            builder.Register<SqlDbTableInspectorStrategy>().As<IDbTableInspectorStrategy>();
        }
    }
}
