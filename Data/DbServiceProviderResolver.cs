using TOF.Core.Configuration;
using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using TOF.Core.Utils;
using TOF.Data.Abstractions;

namespace TOF.Data
{
    //public class DbServiceProviderResolver
    //{
    //    private static Container sServiceResolver = null;
    //    private static IConfiguration _configuration = null;
    //    private static object o = new object();

    //    public static void SetConfiguration(IConfiguration Configuration)
    //    {
    //        ParameterChecker.NotNull(Configuration);

    //        _configuration = Configuration;
    //    }

    //    public static string GetConnectionString()
    //    {
    //        return GetConnectionString("connectionStrings/default");
    //    }

    //    public static string GetConnectionString(string Path)
    //    {
    //        Initialize();

    //        if (_configuration == null)
    //        {
    //            _configuration = sServiceResolver.Resolve<IConfigurationBuilder>().Build();
    //        }

    //        return _configuration[Path];
    //    }

    //    public static IDbServiceProvider GetRuntimeServiceProvider()
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbServiceProvider>(
    //            new NamedParameter("ConnectionString", GetConnectionString()));
    //    }

    //    public static IDbServiceProvider GetRuntimeServiceProviderFromConfigurationPath(string ConfigurationPath)
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbServiceProvider>(
    //            new NamedParameter("ConnectionString", GetConnectionString(ConfigurationPath)));
    //    }

    //    public static IDbServiceProvider GetRuntimeServiceProvider(string ConnectionString)
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbServiceProvider>(
    //            new NamedParameter("ConnectionString", ConnectionString));
    //    }

    //    public static IDbQueryStrategyProvider GetQueryStrategyProvider()
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbQueryStrategyProvider>();
    //    }
        
    //    public static IDbQueryExpressionMemberNameParserProvider GetRuntimeExpressionMemberNameParserProvider()
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbQueryExpressionMemberNameParserProvider>();
    //    }

    //    public static IDbQueryExpressionParserProvider GetRuntimeExpressionParserProvider()
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbQueryExpressionParserProvider>();
    //    }

    //    public static IDbTypeFinder GetRuntimeDbTypeFinder()
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbTypeFinder>();
    //    }

    //    public static IDbEnumTableInspectorStrategy GetEnumTableInspectorStrategy()
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbEnumTableInspectorStrategy>();
    //    }

    //    public static IDbTableInspectorStrategy GetTableInspectorStrategy()
    //    {
    //        Initialize();
    //        return sServiceResolver.Resolve<IDbTableInspectorStrategy>();
    //    }

    //    private static void Initialize()
    //    {
    //        if (sServiceResolver == null)
    //        {
    //            lock (o)
    //            {
    //                if (App.DependencyResolver == null)
    //                    App.Build();
    //            }

    //            sServiceResolver = App.DependencyResolver;
    //        }
    //    }
    //}
}
