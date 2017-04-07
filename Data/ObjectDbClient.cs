using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using TOF.Data.Annotations;
using TOF.Data.Abstractions;
using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;

namespace TOF.Data
{
    public class ObjectDbClient : IDisposable
    {
        private IDictionary<string, IDbStoredProcedureStrategy> _storedProcedureStrategies;
        private readonly string _connectionString;
        private readonly Container _container;

        public ObjectDbClient()
        {
            _container = App.ServiceProviders;
            _connectionString = App.Configuration["connectionStrings/default"];

            Initialize();
        }

        public ObjectDbClient(string connectionString)
        {
            _container = App.ServiceProviders;
            _connectionString = connectionString;

            Initialize();
        }
        
        protected virtual void DefiningDbModelStrategies(DbModelStrategyBuilder builder)
        {
            // currently, do nothing.
        }

        public IDbTable<TEntity> Set<TEntity>(Action<DbModelStrategyBuilder> modelStrategyAction = null)
            where TEntity : class, new()
        {
            var strategyBuilder = new DbModelStrategyBuilder();
            modelStrategyAction?.Invoke(strategyBuilder);

            // get model type for this table.
            var modelType = typeof(TEntity);
            // create default implementation.
            var tableImplType = typeof(DbTable<>).MakeGenericType(modelType);

            // if model strategy is defined, pass it into table constructor.
            var modelStrategy = strategyBuilder.GetTableModelStrategy(modelType);

            // set instance to property.
            if (modelStrategy != null)
                modelStrategy = DetectAndConfigureModelStrategy(modelStrategy);
            else
            {
                modelStrategy = strategyBuilder.Table(modelType);
                modelStrategy = DetectAndConfigureModelStrategy(modelStrategy);
            }

            var dbServiceProvider = GetRuntimeServiceProvider();
            var tableImpl = Activator.CreateInstance(tableImplType, modelStrategy, dbServiceProvider);

            return tableImpl as IDbTable<TEntity>;
        }

        public IDbStoredProcedureInvoker<TModel> Procedure<TModel>(string procedureName)
            where TModel : class, new()
        {
            if (!_storedProcedureStrategies.ContainsKey(procedureName)) return null;

            var dbServiceProvider = GetRuntimeServiceProvider();
            var strategy = _storedProcedureStrategies[procedureName];

            return new DbStoreProcdureInvoker<TModel>(strategy, dbServiceProvider);
        }

        protected DbClient GetDbClient()
        {
            return new DbClient(_connectionString);
        }

        private void Initialize()
        {
            var strategyBuilder = new DbModelStrategyBuilder();
            DefiningDbModelStrategies(strategyBuilder);

            var tableProps = GetType().GetProperties(
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var tableProp in tableProps)
            {
                if (!tableProp.PropertyType.IsGenericType)
                {
                    var modelType = strategyBuilder.GetModelTypeFromProperty(tableProp.Name);

                    // if model strategy is defined, pass it into table constructor.
                    var modelStrategy =
                        strategyBuilder.GetTableModelStrategy(modelType)
                        ?? strategyBuilder.Table(modelType);

                    // detect if model has data annotations.
                    DetectAndConfigureModelStrategy(modelStrategy);

                    // set instance to property.
                    var dbServiceProvider = GetRuntimeServiceProvider();
                    var table = Activator.CreateInstance(typeof(IDbTable), modelStrategy, dbServiceProvider) as IDbTable;
                    tableProp.SetValue(this, table, null);
                }
                else
                {
                    if (!tableProp.PropertyType.FullName.Contains("IDbTable")) continue;

                    var tableInterfaceType = tableProp.PropertyType;

                    // get model type for this table.
                    var modelType = tableInterfaceType.GetGenericArguments().First();
                    // create default implementation.
                    var tableImplType = typeof(DbTable<>).MakeGenericType(modelType);

                    // if model strategy is defined, pass it into table constructor.
                    var modelStrategy = strategyBuilder.GetTableModelStrategy(modelType);

                    // set instance to property.
                    if (modelStrategy != null)
                        modelStrategy = DetectAndConfigureModelStrategy(modelStrategy);
                    else
                    {
                        modelStrategy = strategyBuilder.Table(modelType);
                        modelStrategy = DetectAndConfigureModelStrategy(modelStrategy);
                    }

                    var dbServiceProvider = GetRuntimeServiceProvider();
                    var tableImpl = Activator.CreateInstance(tableImplType, modelStrategy, dbServiceProvider);
                    tableProp.SetValue(this, tableImpl, null);
                }
            }

            _storedProcedureStrategies = strategyBuilder.GetRegisteredStoredProcedureStrategies();
        }

        private static IDbModelStrategy DetectAndConfigureModelStrategy(IDbModelStrategy strategy)
        {
            var modelType = strategy.ModelType;
            var properties = modelType.GetProperties().Where(p => p.DeclaringType != typeof(object));

            var attrTable = GetClassLevelDataAnnotation<TableAttribute>(modelType);

            if (attrTable != null)
                strategy.ChangeTableName(attrTable.TableName);

            foreach (var property in properties)
            {
                var bindingPropertyInfo = strategy.PropertyStrategies
                    .FirstOrDefault(
                    c => c.GetPropertyInfo().PropertyType == property.PropertyType &&
                    c.GetPropertyInfo().DeclaringType == property.DeclaringType &&
                    c.GetPropertyInfo().Name == property.Name)
                    ?? strategy.GetPropertyStrategy(property);

                // allow null attribute
                var attrAllowNull = GetPropertyLevelDataAnnotation<AllowNullAttribute>(property);

                if (attrAllowNull != null && attrAllowNull.AllowNull)
                    bindingPropertyInfo = bindingPropertyInfo.AllowNull();

                // detect nullable type.
                if (property.PropertyType.IsNullable())
                    bindingPropertyInfo.AllowNull();

                // key attribute
                var attrKey = GetPropertyLevelDataAnnotation<KeyAttribute>(property);

                if (attrKey != null)
                {
                    if (bindingPropertyInfo.IsAllowNull())
                        throw new DbKeyNotAllowNullException();

                    bindingPropertyInfo = bindingPropertyInfo.AsKey();
                }

                // ignore attribute.
                var ignoreKey = GetPropertyLevelDataAnnotation<IgnoreAttribute>(property);

                if (ignoreKey != null)
                    bindingPropertyInfo.IgnoreProperty(ignoreKey.IgnoreKinds);

                // DbType attrubyte
                var attrDbType = GetPropertyLevelDataAnnotation<DbTypeAttribute>(property);

                if (attrDbType != null)
                    bindingPropertyInfo = bindingPropertyInfo.MapDbType(attrDbType.DbType);

                if (property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(long) ||
                    property.PropertyType == typeof(short) ||
                    property.PropertyType == typeof(uint) ||
                    property.PropertyType == typeof(ulong) ||
                    property.PropertyType == typeof(ushort))
                {
                    var attrAutoIncremental =
                        GetPropertyLevelDataAnnotation<AutoIncrementalAttribute>(property);

                    if (attrAutoIncremental != null)
                        bindingPropertyInfo = bindingPropertyInfo.AsIncremental();
                }

                // autogenerate attribute.
                if (property.PropertyType != typeof(Guid) && property.PropertyType != typeof(DateTime) &&
                    property.PropertyType != typeof(int) && property.PropertyType != typeof(long) &&
                    property.PropertyType != typeof(short) && property.PropertyType != typeof(uint) &&
                    property.PropertyType != typeof(ulong) && property.PropertyType != typeof(ushort) &&
                    bindingPropertyInfo.GetMapDbType() != DbType.Guid &&
                    bindingPropertyInfo.GetMapDbType() != DbType.DateTime &&
                    bindingPropertyInfo.GetMapDbType() != DbType.Int16 &&
                    bindingPropertyInfo.GetMapDbType() != DbType.Int32 &&
                    bindingPropertyInfo.GetMapDbType() != DbType.Int64 &&
                    bindingPropertyInfo.GetMapDbType() != DbType.UInt16 &&
                    bindingPropertyInfo.GetMapDbType() != DbType.UInt32 &&
                    bindingPropertyInfo.GetMapDbType() != DbType.UInt64) continue;

                var attrAutoGenerated =
                    GetPropertyLevelDataAnnotation<AutoGeneratedAttribute>(property);

                if (attrAutoGenerated == null) continue;

                if (attrAutoGenerated.AtInsert)
                    bindingPropertyInfo = bindingPropertyInfo.AsAutoGenerated();
                if (attrAutoGenerated.AtUpdate)
                    bindingPropertyInfo.AsAutoGeneratedAtUpdate();
            }

            return strategy;
        }

        private static TDataAnnotationAttribute GetClassLevelDataAnnotation<TDataAnnotationAttribute>(Type type)
            where TDataAnnotationAttribute : Attribute
        {
            var dataAnnotationAttribute =
                type.GetCustomAttribute<TDataAnnotationAttribute>(true);

            return dataAnnotationAttribute;
        }

        private static TDataAnnotationAttribute GetPropertyLevelDataAnnotation<TDataAnnotationAttribute>(PropertyInfo property)
            where TDataAnnotationAttribute : Attribute
        {
            var dataAnnotationAttribute =
                property.GetCustomAttribute<TDataAnnotationAttribute>(true);

            return dataAnnotationAttribute;
        }

        private IDbServiceProvider GetRuntimeServiceProvider()
        {
            return string.IsNullOrEmpty(_connectionString)
                ? _container.Resolve<IDbServiceProvider>()
                : _container.Resolve<IDbServiceProvider>(new NamedParameter("ConnectionString", _connectionString));
        }

        public void Dispose()
        {
        }
    }
}
