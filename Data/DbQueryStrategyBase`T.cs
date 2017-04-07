﻿using TOF.Core.DependencyInjection;
using TOF.Core.Infrastructure;
using TOF.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TOF.Data
{
    public abstract class DbQueryStrategyBase<TModel> : IDbQueryStrategy, IDbQueryPagingStrategy 
        where TModel: class, new()
    {
        protected IEnumerable<string> Columns { get; set; }
        protected IDbModelStrategy ModelStrategy { get; set; }
        protected int PageSize { get; set; }
        protected DbQueryStrategyTypes StrategyType { get; private set; }
        protected Container ServiceProvider { get; private set; }

        private DbQueryStrategyBase()
        {
            ServiceProvider = App.ServiceProviders;
        }
        
        public DbQueryStrategyBase(DbQueryStrategyTypes strategyType, int pageSize = 0) : this()
        {
            this.PageSize = pageSize;
            this.StrategyType = strategyType;
            Parse();
        }

        public DbQueryStrategyBase(IDbModelStrategy modelStrategy, DbQueryStrategyTypes strategyType, int pageSize = 0) : this()
        {
            this.ModelStrategy = modelStrategy;
            this.StrategyType = strategyType;
            this.PageSize = pageSize;
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
                    var propQuery = typeof(TModel).GetProperties();

                    foreach (var prop in propQuery)
                    {
                        var modelBindingQuery = modelBindings.Where(
                            c => c.GetPropertyInfo().Name == prop.Name);

                        if (modelBindingQuery.Any())
                        {
                            if (StrategyType == DbQueryStrategyTypes.Update)
                            {
                                if (!modelBindingQuery.First().IsKey() && 
                                    modelBindingQuery.First().IsAutoGenerated() && 
                                    modelBindingQuery.First().IsAutoGeneratedAtUpdate() == false)
                                    continue;
                                if (modelBindingQuery.First().IsIgnoreProperty())
                                {
                                    if (modelBindingQuery.First().GetIgnorePropertyKinds() == DbIgnorePropertyKinds.Update ||
                                        modelBindingQuery.First().GetIgnorePropertyKinds() == DbIgnorePropertyKinds.InsertAndUpdate)
                                        continue;
                                }
                            }
                            else if (StrategyType == DbQueryStrategyTypes.Insert)
                            {
                                if (modelBindingQuery.First().IsAutoGenerated())
                                    continue;
                                if (modelBindingQuery.First().IsIgnoreProperty())
                                {
                                    if (modelBindingQuery.First().GetIgnorePropertyKinds() == DbIgnorePropertyKinds.Insert ||
                                        modelBindingQuery.First().GetIgnorePropertyKinds() == DbIgnorePropertyKinds.InsertAndUpdate)
                                        continue;
                                }
                            }

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
                    var propQuery = typeof(TModel).GetProperties();

                    foreach (var prop in propQuery)
                        columns.Add(prop.Name);
                }
            }
            else
            {
                var propQuery = typeof(TModel).GetProperties();

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
                    var prop = typeof(TModel).GetProperty(column);
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