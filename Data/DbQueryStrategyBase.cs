using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOF.Data.Abstractions;

namespace TOF.Data
{
    public abstract class DbQueryStrategyBase : IDbQueryStrategy, IDbQueryPagingStrategy
    {
        private Type _modelType = null;
        protected IEnumerable<string> Columns { get; set; }
        protected IDbModelStrategy ModelStrategy { get; set; }
        protected int PageSize { get; set; }
        protected Container ServiceProvider { get; private set; }

        private DbQueryStrategyBase()
        {
            ServiceProvider = App.ServiceProviders;
        }

        public DbQueryStrategyBase(Type modelType, int pageSize = 0) : this()
        {
            _modelType = modelType;
            this.PageSize = pageSize;
            Parse();
        }

        public DbQueryStrategyBase(IDbModelStrategy modelStrategy, int pageSize = 0) : this()
        {
            _modelType = modelStrategy.ModelType;
            this.PageSize = pageSize;
            this.ModelStrategy = modelStrategy;
            Parse();
        }

        protected virtual void Parse()
        {
            List<string> columns = new List<string>();

            if (ModelStrategy != null)
            {
                if (ModelStrategy.PropertyStrategies.Any())
                {
                    var modelBindings = ModelStrategy.PropertyStrategies;
                    var propQuery = _modelType.GetProperties()
                        .Where(c => c.DeclaringType == _modelType);

                    foreach (var prop in propQuery)
                    {
                        var modelBindingQuery = modelBindings.Where(
                            c => c.GetPropertyInfo().Name == prop.Name);

                        if (modelBindingQuery.Any())
                        {
                            if (string.IsNullOrEmpty(modelBindingQuery.First().GetParameterName()))
                                columns.Add(prop.Name);
                            else
                                columns.Add(modelBindingQuery.First().GetParameterName());
                        }
                        else
                            columns.Add(prop.Name);
                    }
                }
                else
                {
                    var propQuery = _modelType.GetProperties();

                    foreach (var prop in propQuery)
                        columns.Add(prop.Name);
                }
            }
            else
            {
                var propQuery = _modelType.GetProperties();

                foreach (var prop in propQuery)
                    columns.Add(prop.Name);
            }

            Columns = columns;
        }

        public abstract string GetDbQueryScript();

        public virtual IEnumerable<IDbDataParameter> GetDbParameters()
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();

            if (ModelStrategy == null)
            {
                foreach (var column in Columns)
                {
                    var prop = _modelType.GetProperty(column);
                    var bindingInfo = new DbModelPropertyStrategy(prop);
                    IDbParameterParser parser = new DbParameterParser(bindingInfo);
                    parameters.Add(parser.GetDbParameter());
                }
            }

            return parameters;
        }

        public virtual void DetectQueryRowCount() { /* override only is SELECT strategy. */ }
        public virtual void MoveFirst() { /* override only is SELECT strategy. */ }
        public virtual void MoveLast() { /* override only is SELECT strategy. */ }
        public virtual void MoveNext() { /* override only is SELECT strategy. */ }
        public virtual void MovePrevious() { /* override only is SELECT strategy. */ }
    }
}
