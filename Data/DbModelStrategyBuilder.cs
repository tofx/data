using System;
using System.Collections.Generic;
using System.Linq;
using TOF.Data.Abstractions;
using TOF.Data.Annotations;

namespace TOF.Data
{
    public class DbModelStrategyBuilder
    {
        private readonly IDictionary<Type, object> _modelStrategies;
        private readonly IDictionary<string, IDbStoredProcedureStrategy> _storedProcedureStrategies;
        private readonly IDictionary<string, Type> _bindingModelTypes;

        public DbModelStrategyBuilder()
        {
            _modelStrategies = new Dictionary<Type, object>();
            _storedProcedureStrategies = new Dictionary<string, IDbStoredProcedureStrategy>();
            _bindingModelTypes = new Dictionary<string, Type>();
        }

        public IDbModelStrategy GetTableStrategy(string tableName)
        {
            var strategyQuery = _modelStrategies.Where(c =>
                ((IDbModelStrategy)c.Value).DbTableName == tableName).ToList();

            return (strategyQuery.Any()) ? (IDbModelStrategy)strategyQuery.First().Value : null;
        }

        public IDbModelStrategy GetTableModelStrategy(Type modelType)
        {
            return (_modelStrategies.ContainsKey(modelType)) ? (IDbModelStrategy)_modelStrategies[modelType] : null;
        }

        public IDbModelStrategy GetTableModelStrategy<TModel>()
        {
            return GetTableModelStrategy(typeof(TModel));
        }

        public void BindingModelTypeToProperty(string propertyName, Type modelType)
        {
            _bindingModelTypes.Add(propertyName, modelType);
        }

        public Type GetModelTypeFromProperty(string propertyName)
        {
            return _bindingModelTypes[propertyName];
        }

        public IDictionary<string, IDbStoredProcedureStrategy> GetRegisteredStoredProcedureStrategies()
        {
            return _storedProcedureStrategies;
        }
        
        public IDbModelStrategy Table(Type modelType)
        {
            var tableName = modelType.Name;
            var attrTableNames = (TableAttribute[])modelType.GetCustomAttributes(typeof(TableAttribute), true);

            if (attrTableNames.Length > 0)
                tableName = attrTableNames[0].TableName;

            var modelStrategy = new DbModelStrategy(modelType, tableName);

            if (_modelStrategies.ContainsKey(modelType))
            {
                return _modelStrategies[modelType] as IDbModelStrategy;
            }

            _modelStrategies.Add(modelType, modelStrategy);
            return modelStrategy;
        }

        public IDbModelStrategy Table(Type modelType, string tableName)
        {
            var modelStrategy = new DbModelStrategy(modelType, tableName);

            if (_modelStrategies.ContainsKey(modelType))
                return (IDbModelStrategy)_modelStrategies[modelType];

            _modelStrategies.Add(modelType, modelStrategy);
            return modelStrategy;
        }

        public IDbModelStrategy Table(Type modelType, string tableName, string tableAlias)
        {
            var modelStrategy = new DbModelStrategy(modelType, tableName, tableAlias);

            if (_modelStrategies.ContainsKey(modelType))
                return _modelStrategies[modelType] as IDbModelStrategy;

            _modelStrategies.Add(modelType, modelStrategy);
            return modelStrategy;
        }

        public IDbModelStrategy<TModel> Table<TModel>() where TModel : class, new()
        {
            var tableName = typeof(TModel).Name;
            var attrTableNames = (TableAttribute[])typeof(TModel).GetCustomAttributes(typeof(TableAttribute), true);

            if (attrTableNames.Length > 0)
                tableName = attrTableNames[0].TableName;

            IDbModelStrategy<TModel> modelStrategy = new DbModelStrategy<TModel>(tableName);

            if (_modelStrategies.ContainsKey(typeof(TModel)))
            {
                if (_modelStrategies[typeof(TModel)] == null)
                {
                    _modelStrategies[typeof(TModel)] = modelStrategy;
                    return modelStrategy;
                }

                if (TryConvertModelStrategy(_modelStrategies[typeof(TModel)], out modelStrategy))
                {
                    modelStrategy = (IDbModelStrategy<TModel>)_modelStrategies[typeof(TModel)];
                    modelStrategy.ChangeTableName(tableName);
                    _modelStrategies[typeof(TModel)] = modelStrategy;
                    return modelStrategy;
                }

                _modelStrategies[typeof(TModel)] = modelStrategy;
                return modelStrategy;
            }

            _modelStrategies.Add(typeof(TModel), modelStrategy);
            return modelStrategy;
        }

        public IDbModelStrategy<TModel> Table<TModel>(string tableName) where TModel : class, new()
        {
            IDbModelStrategy<TModel> modelStrategy = new DbModelStrategy<TModel>(tableName);

            if (_modelStrategies.ContainsKey(typeof(TModel)))
            {
                if (_modelStrategies[typeof(TModel)] == null)
                {
                    _modelStrategies[typeof(TModel)] = modelStrategy;
                    return modelStrategy;
                }

                if (TryConvertModelStrategy(_modelStrategies[typeof(TModel)], out modelStrategy))
                {
                    modelStrategy = (IDbModelStrategy<TModel>)_modelStrategies[typeof(TModel)];
                    modelStrategy.ChangeTableName(tableName);
                    _modelStrategies[typeof(TModel)] = modelStrategy;
                    return modelStrategy;
                }

                _modelStrategies[typeof(TModel)] = modelStrategy;
                return modelStrategy;
            }

            _modelStrategies.Add(typeof(TModel), modelStrategy);
            return modelStrategy;
        }

        public IDbModelStrategy<TModel> Table<TModel>(string tableName, string tableAlias) where TModel : class, new()
        {
            IDbModelStrategy<TModel> modelStrategy = new DbModelStrategy<TModel>(tableName, tableAlias);

            if (_modelStrategies.ContainsKey(typeof(TModel)))
            {
                if (_modelStrategies[typeof(TModel)] == null)
                {
                    _modelStrategies[typeof(TModel)] = modelStrategy;
                    return modelStrategy;
                }

                if (TryConvertModelStrategy(_modelStrategies[typeof(TModel)], out modelStrategy))
                {
                    modelStrategy = (IDbModelStrategy<TModel>)_modelStrategies[typeof(TModel)];
                    modelStrategy.ChangeTableName(tableName, tableAlias);
                    _modelStrategies[typeof(TModel)] = modelStrategy;
                    return modelStrategy;
                }

                _modelStrategies[typeof(TModel)] = modelStrategy;
                return modelStrategy;
            }

            _modelStrategies.Add(typeof(TModel), modelStrategy);
            return modelStrategy;
        }

        public IDbStoredProcedureStrategy Procedure(Type modelType, string procedureName)
        {
            var strategy = new DbStoredProcedureStrategy(modelType, procedureName);
            _storedProcedureStrategies.Add(procedureName, strategy);
            return strategy;
        }

        public IDbStoredProcedureStrategy<TModel> Procedure<TModel>(string procedureName) where TModel : class, new()
        {
            var strategy = new DbStoredProcedureStrategy<TModel>(procedureName);
            _storedProcedureStrategies.Add(procedureName, strategy);
            return strategy;
        }

        private static bool TryConvertModelStrategy<TModel>(object item, out IDbModelStrategy<TModel> strategy)
            where TModel : class, new()
        {
            strategy = null;

            var value = item as IDbModelStrategy<TModel>;

            if (value == null) return false;

            strategy = value;
            return true;
        }
    }
}