using tofx.Core.DependencyInjection;
using tofx.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tofx.Data.Abstractions;

namespace tofx.Data
{
    public class DbExecuteContext<TModel> : IDbExecuteContext<TModel> where TModel : class, new()
    {
        private IDbTable<TModel> _queryTable = null;
        private IDbServiceProvider _serviceProvider = null;
        private IDbQueryStrategyProvider _queryStrategyProvider = null;
        private Container _container = null;

        private DbExecuteContext()
        {
            _container = App.ServiceProviders;
        }
        
        public DbExecuteContext(IDbTable<TModel> dbTable) : this()
        {
            _queryTable = dbTable;
            _serviceProvider = _container.Resolve<IDbServiceProvider>();
            _queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
        }

        public DbExecuteContext(IDbTable<TModel> dbTable, string connectionString) : this()
        {
            _queryTable = dbTable;
            _serviceProvider = _container.Resolve<IDbServiceProvider>();
            _queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
        }

        public DbExecuteContext(IDbTable<TModel> dbTable, IDbServiceProvider runtimeProvider) : this()
        {
            _queryTable = dbTable;
            _serviceProvider = runtimeProvider;
            _queryStrategyProvider = _container.Resolve<IDbQueryStrategyProvider>();
        }

        public DbExecuteContext(
            IDbTable<TModel> dbTable, IDbServiceProvider runtimeProvider, IDbQueryStrategyProvider queryStrategyProvider) : this()
        {
            _queryTable = dbTable;
            _serviceProvider = runtimeProvider;
            _queryStrategyProvider = queryStrategyProvider;
        }

        public int Delete(IEnumerable<TModel> models, bool throwIfAnyModelFailed = true)
        {
            int success = 0;

            var queryStrategy = _queryStrategyProvider.GetDeleteStrategy<TModel>(_queryTable.ModelStrategy);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters();
            var exceptions = new List<DbOperationException>();

            _serviceProvider.Open();

            foreach (var model in models)
            {
                DbModelBindingHelper.BindingModelToParameters(
                    model, _queryTable.ModelStrategy.PropertyStrategies, dbparams);

                try
                {
                    if (_serviceProvider.Execute(sql, dbparams) > 0)
                        success++;
                    else
                        exceptions.Add(new DbOperationException("ERROR_SQL_EXECUTION_FAILED", sql, dbparams));
                }
                catch (Exception ex)
                {
                    exceptions.Add(new DbOperationException("ERROR_SQL_EXECUTION_FAILED", ex, sql, dbparams));
                }
            }

            _serviceProvider.Close();

            if (throwIfAnyModelFailed)
                throw new DbMultipleOperationsException(exceptions);

            return success;
        }

        public void Delete(TModel model)
        {
            var queryStrategy = _queryStrategyProvider.GetDeleteStrategy<TModel>(_queryTable.ModelStrategy);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters();

            DbModelBindingHelper.BindingModelToParameters(
                model, _queryTable.ModelStrategy.PropertyStrategies, dbparams);

            try
            {
                _serviceProvider.Open();

                if (_serviceProvider.Execute(sql, dbparams) == 0)
                    throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", sql, dbparams);
            }
            catch (Exception exception)
            {
                throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", exception, sql, dbparams);
            }
            finally
            {
                _serviceProvider.Close();
            }
        }

        public int Insert(IEnumerable<TModel> models, bool throwIfAnyModelFailed = true)
        {
            int success = 0;

            var queryStrategy = _queryStrategyProvider.GetInsertStrategy<TModel>(_queryTable.ModelStrategy);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters();
            var exceptions = new List<DbOperationException>();

            _serviceProvider.Open();

            foreach (var model in models)
            {
                DbModelBindingHelper.BindingModelToParameters(
                    model, _queryTable.ModelStrategy.PropertyStrategies, dbparams);

                try
                {
                    if (_serviceProvider.Execute(sql, dbparams) > 0)
                        success++;
                    else
                        exceptions.Add(new DbOperationException("ERROR_SQL_EXECUTION_FAILED", sql, dbparams));
                }
                catch (Exception ex)
                {
                    exceptions.Add(new DbOperationException("ERROR_SQL_EXECUTION_FAILED", ex, sql, dbparams));
                }
            }

            _serviceProvider.Close();

            if (throwIfAnyModelFailed)
                throw new DbMultipleOperationsException(exceptions);

            return success;
        }

        public void Insert(TModel model)
        {
            var queryStrategy = _queryStrategyProvider.GetInsertStrategy<TModel>(_queryTable.ModelStrategy);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters();

            DbModelBindingHelper.BindingModelToParameters(
                model, _queryTable.ModelStrategy.PropertyStrategies, dbparams);

            try
            {
                _serviceProvider.Open();

                if (_serviceProvider.Execute(sql, dbparams) == 0)
                    throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", sql, dbparams);
            }
            catch (Exception exception)
            {
                throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", exception, sql, dbparams);
            }
            finally
            {
                _serviceProvider.Close();
            }
        }

        public int Update(IEnumerable<TModel> models, bool throwIfAnyModelFailed = true)
        {
            int success = 0;

            var queryStrategy = _queryStrategyProvider.GetUpdateStrategy<TModel>(_queryTable.ModelStrategy);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters();
            var exceptions = new List<DbOperationException>();

            _serviceProvider.Open();

            foreach (var model in models)
            {
                DbModelBindingHelper.BindingModelToParameters(
                    model, _queryTable.ModelStrategy.PropertyStrategies, dbparams);

                try
                {
                    if (_serviceProvider.Execute(sql, dbparams) > 0)
                        success++;
                    else
                        exceptions.Add(new DbOperationException("ERROR_SQL_EXECUTION_FAILED", sql, dbparams));
                }
                catch (Exception ex)
                {
                    exceptions.Add(new DbOperationException("ERROR_SQL_EXECUTION_FAILED", ex, sql, dbparams));
                }
            }

            _serviceProvider.Close();

            if (throwIfAnyModelFailed)
                throw new DbMultipleOperationsException(exceptions);

            return success;
        }

        public void Update(TModel model)
        {
            var queryStrategy = _queryStrategyProvider.GetUpdateStrategy<TModel>(_queryTable.ModelStrategy);
            var sql = queryStrategy.GetDbQueryScript();
            var dbparams = queryStrategy.GetDbParameters();

            DbModelBindingHelper.BindingModelToParameters(
                model, _queryTable.ModelStrategy.PropertyStrategies, dbparams);

            try
            {
                _serviceProvider.Open();

                if (_serviceProvider.Execute(sql, dbparams) == 0)
                    throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", sql, dbparams);
            }
            catch (Exception exception)
            {
                throw new DbOperationException("ERROR_SQL_EXECUTION_FAILED", exception, sql, dbparams);
            }
            finally
            {
                _serviceProvider.Close();
            }
        }
    }
}
