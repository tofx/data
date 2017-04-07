using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using TOF.Data.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TOF.Data
{
    public class DbTable : IDbTable
    {
        private readonly IDbModelStrategy _modelStrategy;
        private readonly IDbServiceProvider _serviceProvider;
        private readonly Container _container;

        public DbTable()
        {
            _container = App.ServiceProviders;
        }

        public DbTable(IDbModelStrategy modelStrategy, IDbServiceProvider dbServiceProvider) : this()
        {
            _modelStrategy = modelStrategy;
            _serviceProvider = dbServiceProvider;
        }

        public IDbModelStrategy ModelStrategy => _modelStrategy;
        public bool IsView => false;
        public string Name => _modelStrategy.DbTableName;
        public DbObjectTypes Type => DbObjectTypes.Table;

        public void Delete<TModel>(TModel model) where TModel : class, new()
        {
            var queryStrategyProvider = App.ServiceProviders.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetDeleteStrategy<TModel>(_modelStrategy);

            ExecuteModelDbOperation(strategy, model);
        }

        public void Delete<TModel>(IEnumerable<TModel> models) where TModel : class, new()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetDeleteStrategy<TModel>(_modelStrategy);

            ExecuteModelDbOperation(strategy, models);
        }

        public void Insert<TModel>(TModel model) where TModel : class, new()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetInsertStrategy<TModel>(_modelStrategy);

            ExecuteModelDbOperation(strategy, model);
        }

        public void Insert<TModel>(IEnumerable<TModel> models) where TModel : class, new()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetInsertStrategy<TModel>(_modelStrategy);

            ExecuteModelDbOperation(strategy, models);
        }

        public void Update<TModel>(TModel model) where TModel : class, new()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetUpdateStrategy<TModel>(_modelStrategy);

            ExecuteModelDbOperation(strategy, model);
        }

        public void Update<TModel>(IEnumerable<TModel> models) where TModel : class, new()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetUpdateStrategy<TModel>(_modelStrategy);

            ExecuteModelDbOperation(strategy, models);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Empty()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetEmptyStrategy(_modelStrategy);

            _serviceProvider.Open();
            _serviceProvider.Execute(strategy.GetDbQueryScript(), new List<IDbDataParameter>());
            _serviceProvider.Close();
        }

        public void DeleteAll()
        {
            var queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
            var strategy = queryStrategyProvider.GetDeleteAllStrategy(_modelStrategy);

            _serviceProvider.Open();
            _serviceProvider.Execute(strategy.GetDbQueryScript(), new List<IDbDataParameter>());
            _serviceProvider.Close();
        }

        private void ExecuteModelDbOperation<TModel>(IDbQueryStrategy strategy, TModel model) where TModel : class, new()
        {
            Exception exception = null;
            var dbparams = DbModelBindingHelper.BindingModelToParameters(
                model, _modelStrategy.PropertyStrategies, strategy.GetDbParameters());

            _serviceProvider.Open();

            try
            {
                if (_serviceProvider.Execute(strategy.GetDbQueryScript(), dbparams) == 0)
                    throw new DbChangeNoAffectException();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            _serviceProvider.Close();

            if (exception != null)
                throw new DbAggregateException(exception);
        }
        
        private void ExecuteModelDbOperation<TModel>(IDbQueryStrategy strategy, IEnumerable<TModel> models) where TModel : class, new()
        {
            var exceptions = new Dictionary<int, Exception>();

            _serviceProvider.Open();
            _serviceProvider.BeginTransaction();

            var modelIdx = 0;

            foreach (var model in models)
            {
                try
                {
                    var dbparams = DbModelBindingHelper.BindingModelToParameters(
                        model, _modelStrategy.PropertyStrategies, strategy.GetDbParameters());
                    if (_serviceProvider.Execute(strategy.GetDbQueryScript(), dbparams) == 0)
                        throw new DbChangeNoAffectException();
                }
                catch (Exception ex)
                {
                    exceptions.Add(modelIdx, ex);
                }

                modelIdx++;
            }

            if (!exceptions.Any())
            {
                _serviceProvider.Commit();
                _serviceProvider.Close();
            }
            else
            {
                _serviceProvider.Rollback();
                _serviceProvider.Close();
                throw new DbAggregateException(exceptions);
            }
        }
    }
}
