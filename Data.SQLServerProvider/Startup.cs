using TOF.Core.DependencyInjection;
using TOF.Data.Abstractions;
using TOF.Data.Providers.SqlServer.InspectorStrategies;

[assembly: Startup(typeof(TOF.Data.Providers.SqlServer.Startup))]

namespace TOF.Data.Providers.SqlServer
{
    public class Startup
    {
        public void Initialize(ContainerBuilder builder)
        {
            // SQL Server Runtime Provider.
            builder.Register<SqlDbServiceProvider>()
                .As<IDbServiceProvider>()
                .As<IDbTransactionProvider>()
                .OnActivating((c, p) =>
                {
                    return new SqlDbServiceProvider(p.Named<string>("ConnectionString"));
                });

            builder.Register<SqlDbQueryStrategyProvider>().As<IDbQueryStrategyProvider>();
            builder.Register<SqlDbParamChainProvider>().As<IDbParamChainProvider>().AsSingleInstance();
            builder.Register<SqlDbQueryExpressionMemberNameParserProvider>().As<IDbQueryExpressionMemberNameParserProvider>().AsSingleInstance();
            builder.Register<SqlDbQueryExpressionParserProvider>().As<IDbQueryExpressionParserProvider>().AsSingleInstance();
            builder.Register<SqlDbTypeFinderProvider>().As<IDbTypeFinderProvider>().AsSingleInstance();
            builder.Register<SqlDbSchemaStrategyProvider>().As<IDbSchemaStrategyProvider>();
            //builder.Register<SqlDbQueryAddExpressionNode>().As<IDbQueryExpressionNode>();
            builder.Register<SqlDbEnumTableInspectorStrategy>().As<IDbEnumTableInspectorStrategy>();
            builder.Register<SqlDbTableInspectorStrategy>().As<IDbTableInspectorStrategy>();
            builder.Register<SqlDbQueryParameterParsingStrategyFactory>().As<IDbQueryParameterParsingStrategyFactory>().AsSingleInstance();
        }
    }
}
