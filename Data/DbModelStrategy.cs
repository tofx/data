using tofx.Core.DependencyInjection;
using tofx.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using tofx.Core.Utils.TypeExtensions;
using tofx.Data.Abstractions;

namespace tofx.Data
{
    public class DbModelStrategy : IDbModelStrategy
    {
        private readonly List<IDbModelPropertyStrategy> _modelPropertyBindings;
        private readonly Container _container;
        private string _tableAlias;

        // C/U/D operation via stored procedure strategy.
        private IDbStoredProcedureStrategy _insertProcStrategy;
        private IDbStoredProcedureStrategy _updateProcStrategy;
        private IDbStoredProcedureStrategy _deleteProcStrategy;

        private DbModelStrategy()
        {
            _container = App.ServiceProviders;
        }

        public DbModelStrategy(
            Type modelType, string tableName, string tableAlias = null,
            IEnumerable<IDbModelPropertyStrategy> modelPropertyBindings = null) : this()
        {
            ModelType = modelType;
            DbTableName = tableName;

            _tableAlias = tableAlias.IsNullOrEmpty()
                ? _tableAlias = tableAlias 
                :_tableAlias = DbTableName.ToLower();

            _modelPropertyBindings = (modelPropertyBindings == null)
                ? new List<IDbModelPropertyStrategy>()
                : new List<IDbModelPropertyStrategy>(modelPropertyBindings);

            Initialize();
        }

        private void Initialize()
        {
            // create default property binding information.
            var propQuery = ModelType.GetProperties();

            if (!propQuery.Any())
                return;

            foreach (var prop in propQuery)
            {
                var propBindingInfo = new DbModelPropertyStrategy(prop);
                AddOrReplacePropertyBindingInfo(prop.Name, propBindingInfo);
            }
        }

        public string DbTableName { get; private set; }
        public Type ModelType { get; }

        public IEnumerable<IDbModelPropertyStrategy> PropertyStrategies => _modelPropertyBindings;

        public IDbModelPropertyStrategy Property<T>(Expression<Func<T, object>> propertySpecifier)
        {
            if (typeof(T) != ModelType)
                throw new ArgumentException("ERROR_MODEL_TYPE_MISMATCH");

            var parserProvider = _container.Resolve<IDbQueryExpressionMemberNameParserProvider>();

            if (parserProvider == null)
                throw new InvalidOperationException("DbServiceProvider can't find query expression member name parser provider.");

            var expressionNode = parserProvider.GetParser(this);
            string propertyName = expressionNode.Parse(new Dictionary<string, object>(), propertySpecifier.Body);

            var prop = typeof(T).GetProperty(propertyName);

            if (prop == null)
                throw new ArgumentException("ERROR_PROPERTY_NOT_FOUND");

            IDbModelPropertyStrategy propertyBindingInfo = new DbModelPropertyStrategy(prop);
            AddOrReplacePropertyBindingInfo(prop.Name, propertyBindingInfo);
            return propertyBindingInfo;
        }

        public IDbModelPropertyStrategy PropertyExact(PropertyInfo property)
        {
            IDbModelPropertyStrategy propertyBindingInfo = new DbModelPropertyStrategy(property);
            AddOrReplacePropertyBindingInfo(property.Name, propertyBindingInfo);
            return propertyBindingInfo;
        }

        public IDbModelPropertyStrategy GetPropertyStrategy(PropertyInfo property)
        {
            var query = _modelPropertyBindings.Where(r => r.GetPropertyInfo() == property).ToList();
            return (query.Any()) ? query.First() : null;
        }

        public IDbStoredProcedureStrategy InsertProcedure(string procedureName)
        {
            _insertProcStrategy = new DbStoredProcedureStrategy(ModelType, procedureName);
            return _insertProcStrategy;
        }

        public IDbStoredProcedureStrategy UpdateProcedure(string procedureName)
        {
            _updateProcStrategy = new DbStoredProcedureStrategy(ModelType, procedureName);
            return _updateProcStrategy;
        }

        public IDbStoredProcedureStrategy DeleteProcedure(string procedureName)
        {
            _deleteProcStrategy = new DbStoredProcedureStrategy(ModelType, procedureName);
            return _deleteProcStrategy;
        }

        public IDbStoredProcedureStrategy GetInsertProcedure()
        {
            return _insertProcStrategy;
        }

        public IDbStoredProcedureStrategy GetUpdateProcedure()
        {
            return _updateProcStrategy;
        }

        public IDbStoredProcedureStrategy GetDeleteProcedure()
        {
            return _deleteProcStrategy;
        }

        private void AddOrReplacePropertyBindingInfo(string propertyName, IDbModelPropertyStrategy modelPropertyStrategy)
        {
            if (_modelPropertyBindings.Any(c => c.GetPropertyInfo().Name == propertyName))
            {
                _modelPropertyBindings.Remove(_modelPropertyBindings.First(c => c.GetPropertyInfo().Name == propertyName));
            }

            _modelPropertyBindings.Add(modelPropertyStrategy);
        }

        public void ChangeTableName(string tableName, string tableAlias = null)
        {
            DbTableName = tableName;
            _tableAlias = tableAlias;
        }
    }
}
