using tofx.Core.DependencyInjection;
using tofx.Core.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using tofx.Data.Abstractions;

namespace tofx.Data.Schema
{
    public static class DbSchemaInitializer
    {
        public static void InitializeDbModelSchemas(IEnumerable<Assembly> ProbeAssemblies)
        {
            if (ProbeAssemblies == null || ProbeAssemblies.Count() == 0)
                return;

            //var connectionString = GetConnectionString();
            //var schemaDefinitionStrategyProvider = GetSchemaDefinitionStrategyProvider();

            //foreach (var assembly in ProbeAssemblies)
            //{
            //    var objectTypes = assembly.GetTypes().Where(t => t.GetInterface("IDbSchemaSyncRegistration") != null);

            //    foreach (var type in objectTypes)
            //    {
            //        var schemaSyncRegistration = Activator.CreateInstance(type) as IDbSchemaRegistration;

            //        foreach (var modelStrategy in schemaSyncRegistration.GetSchemaRegistrations())
            //        {
            //            var strategies = new List<IDbSchemaStrategy>()
            //                {
            //                    schemaDefinitionStrategyProvider.GetCreateTableStrategy(modelStrategy),
            //                    schemaDefinitionStrategyProvider.GetAlterTableStrategy(modelStrategy),
            //                    schemaDefinitionStrategyProvider.GetDropTableStrategy(modelStrategy),
            //                    schemaDefinitionStrategyProvider.GetLookupSchemaExistsStrategy(modelStrategy)
            //                };

            //            var schemaSyncProvider =
            //                GetSchemaGenerationProvider(modelStrategy, strategies, connectionString);

            //            schemaSyncProvider.Create();
            //        }
            //    }
            //}
        }

        private static string GetConnectionString(string ConnectionStringName = "default")
        {
            return App.Configuration["connectionStrings/" + ConnectionStringName];
        }

        private static IDbSchemaActionProvider GetSchemaGenerationProvider(
            IDbModelStrategy ModelStrategy, IEnumerable<IDbSchemaStrategy> SchemaGenerationStrategies, string ConnectionString)
        {
            return App.ServiceProviders.Resolve<IDbSchemaActionProvider>(
                new NamedParameter("ModelStrategy", ModelStrategy),
                new NamedParameter("SchemaGenerationStrategies", SchemaGenerationStrategies),
                new NamedParameter("ConnectionString", ConnectionString));
        }

        private static IDbSchemaStrategyProvider GetSchemaDefinitionStrategyProvider()
        {
            return App.ServiceProviders.Resolve<IDbSchemaStrategyProvider>();
        }
    }
}
