using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TOF.Core.Utils.TypeExtensions;
using TOF.Data.Abstractions;

namespace TOF.Data
{
    public class DbModelStrategy<TModel> : IDbModelStrategy<TModel> where TModel: class, new()
    {
        private string _tableAlias;
        private readonly List<IDbModelPropertyStrategy> _modelPropertyBindings ;
        private readonly Container _container;

        // C/U/D operation via stored procedure strategy.
        private IDbStoredProcedureStrategy<TModel> _insertProcStrategy;
        private IDbStoredProcedureStrategy<TModel> _updateProcStrategy;
        private IDbStoredProcedureStrategy<TModel> _deleteProcStrategy;

        private DbModelStrategy()
        {
            _container = App.ServiceProviders;
        }

        public DbModelStrategy(
            string tableName, string tableAlias = null,
            IEnumerable<IDbModelPropertyStrategy> modelPropertyBindings = null) : this()
        {
            DbTableName = tableName;

            _tableAlias = (!tableAlias.IsNullOrEmpty()) ?  tableAlias :DbTableName.ToLower();

            _modelPropertyBindings = (modelPropertyBindings == null)
                ? new List<IDbModelPropertyStrategy>()
                : new List<IDbModelPropertyStrategy>(modelPropertyBindings);

            Initialize();
        }

        private void Initialize()
        {
            // create default property binding information.
            var propQuery = typeof(TModel).GetProperties();

            if (!propQuery.Any())
                return;

            foreach (var prop in propQuery)
            {
                var propBindingInfo = new DbModelPropertyStrategy(prop);
                AddOrReplacePropertyBindingInfo(prop.Name, propBindingInfo);
            }
        }

        public string DbTableName { get; private set; }
        public Type ModelType => typeof(TModel);

        public IEnumerable<IDbModelPropertyStrategy> PropertyStrategies => _modelPropertyBindings;

        public IDbModelPropertyStrategy Property(Expression<Func<TModel, object>> propertySpecifier)
        {
            var parserProvider = _container.Resolve<IDbQueryExpressionMemberNameParserProvider>();

            if (parserProvider == null)
                throw new InvalidOperationException("DbServiceProvider can't find query expression member name parser provider.");

            var expressionNode = parserProvider.GetParser(this);
            string propertyName = expressionNode.Parse(new Dictionary<string, object>(), propertySpecifier.Body);

            var prop = typeof(TModel).GetProperty(propertyName);

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

        public IDbStoredProcedureStrategy<TModel> InsertProc(string procedureName)
        {
            _insertProcStrategy = new DbStoredProcedureStrategy<TModel>(procedureName);
            return _insertProcStrategy;
        }

        public IDbStoredProcedureStrategy<TModel> UpdateProc(string procedureName)
        {
            _updateProcStrategy = new DbStoredProcedureStrategy<TModel>(procedureName);
            return _updateProcStrategy;
        }
        public IDbStoredProcedureStrategy<TModel> DeleteProc(string procedureName)
        {
            _deleteProcStrategy = new DbStoredProcedureStrategy<TModel>(procedureName);
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
        
        private void AddOrReplacePropertyBindingInfo(string propertyName, IDbModelPropertyStrategy propertyBindingInfo)
        {
            if (_modelPropertyBindings.Any(c => c.GetPropertyInfo().Name == propertyName))
            {
                _modelPropertyBindings.Remove(
                    _modelPropertyBindings.First(c => c.GetPropertyInfo().Name == propertyName));
            }

            _modelPropertyBindings.Add(propertyBindingInfo);
        }

        public void ChangeTableName(string tableName, string tableAlias = null)
        {
            DbTableName = tableName;
            _tableAlias = tableAlias;
        }
    }
}
